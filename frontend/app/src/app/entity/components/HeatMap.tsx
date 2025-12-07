"use client";

import { useEffect, useRef } from "react";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import "leaflet.heat";

interface Incident {
  id: number | string;
  title: string;
  description: string;
  department: string;
  municipality: string;
  date: string;
  priority: string;
  status: string;
  coordinates: { lat: number; lng: number };
}

interface HeatMapProps {
  incidents: Incident[];
  onBoundsChange?: (visibleIncidentIds: (number | string)[]) => void;
}

function HeatMap({ incidents, onBoundsChange }: HeatMapProps) {
  const mapRef = useRef<L.Map | null>(null);
  const heatLayerRef = useRef<L.HeatLayer | null>(null);
  const markersRef = useRef<L.LayerGroup | null>(null);
  const onBoundsChangeRef = useRef(onBoundsChange);
  const incidentsRef = useRef(incidents);

  // Mantener las referencias actualizadas
  useEffect(() => {
    onBoundsChangeRef.current = onBoundsChange;
  }, [onBoundsChange]);

  useEffect(() => {
    incidentsRef.current = incidents;
  }, [incidents]);

  useEffect(() => {
    // Inicializar el mapa solo una vez
    if (!mapRef.current) {
      const map = L.map("map").setView([4.5709, -74.2973], 6); // Centro de Colombia

      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution:
          '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 18,
      }).addTo(map);

      mapRef.current = map;
      markersRef.current = L.layerGroup().addTo(map);

      // Agregar listeners para detectar cuando el usuario mueve o hace zoom en el mapa
      const updateVisibleIncidents = () => {
        if (!mapRef.current || !onBoundsChangeRef.current) return;
        
        const bounds = mapRef.current.getBounds();
        const visible = incidentsRef.current.filter(inc => 
          bounds.contains([inc.coordinates.lat, inc.coordinates.lng])
        ).map(inc => inc.id);
        
        onBoundsChangeRef.current(visible);
      };

      map.on('moveend', updateVisibleIncidents);
      map.on('zoomend', updateVisibleIncidents);
    }

    return () => {
      // Cleanup solo cuando el componente se desmonta completamente
      if (mapRef.current) {
        mapRef.current.remove();
        mapRef.current = null;
      }
    };
  }, []);

  useEffect(() => {
    if (!mapRef.current) return;

    // Limpiar capa de calor anterior
    if (heatLayerRef.current) {
      mapRef.current.removeLayer(heatLayerRef.current);
    }

    // Limpiar marcadores anteriores
    if (markersRef.current) {
      markersRef.current.clearLayers();
    }

    if (incidents.length === 0) return;

    // Preparar datos para el mapa de calor
    // Formato: [lat, lng, intensidad]
    const heatData: [number, number, number][] = incidents.map((incident) => {
      // La intensidad depende de la prioridad
      const intensity =
        incident.priority === "critical"
          ? 1.0
          : incident.priority === "high"
          ? 0.7
          : incident.priority === "medium"
          ? 0.5
          : 0.3;

      return [incident.coordinates.lat, incident.coordinates.lng, intensity];
    });

    // Crear capa de calor
    const heatLayer = L.heatLayer(heatData, {
      radius: 25,
      blur: 35,
      maxZoom: 10,
      max: 1.0,
      gradient: {
        0.0: "blue",
        0.3: "lime",
        0.5: "yellow",
        0.7: "orange",
        1.0: "red",
      },
    }).addTo(mapRef.current);

    heatLayerRef.current = heatLayer;

    // Agregar marcadores individuales
    incidents.forEach((incident) => {
      const color =
        incident.priority === "critical"
          ? "red"
          : incident.priority === "high"
          ? "orange"
          : incident.priority === "medium"
          ? "yellow"
          : "blue";

      const marker = L.circleMarker(
        [incident.coordinates.lat, incident.coordinates.lng],
        {
          radius: 8,
          fillColor: color,
          color: "#fff",
          weight: 2,
          opacity: 1,
          fillOpacity: 0.8,
        }
      );

      marker.bindPopup(`
        <div style="min-width: 200px;">
          <h3 style="font-weight: bold; margin-bottom: 8px;">${incident.title}</h3>
          <p style="margin-bottom: 4px; font-size: 12px;">${incident.description}</p>
          <p style="margin-bottom: 4px; font-size: 12px;"><strong>Departamento:</strong> ${incident.department}</p>
          <p style="margin-bottom: 4px; font-size: 12px;"><strong>Municipio:</strong> ${incident.municipality}</p>
          <p style="margin-bottom: 4px; font-size: 12px;"><strong>Fecha:</strong> ${incident.date}</p>
        </div>
      `);

      if (markersRef.current) {
        marker.addTo(markersRef.current);
      }
    });

    // Ajustar el zoom para mostrar todos los incidentes
    if (incidents.length > 0) {
      const bounds = L.latLngBounds(
        incidents.map((inc) => [inc.coordinates.lat, inc.coordinates.lng])
      );
      mapRef.current.fitBounds(bounds, { padding: [50, 50] });
    }
  }, [incidents]);

  // Actualizar incidentes visibles cuando cambian los incidentes
  useEffect(() => {
    if (!mapRef.current || !onBoundsChangeRef.current || incidents.length === 0) return;

    // Pequeño delay para evitar llamadas durante el render
    const timeoutId = setTimeout(() => {
      if (!mapRef.current || !onBoundsChangeRef.current) return;
      
      const bounds = mapRef.current.getBounds();
      const visible = incidentsRef.current.filter(inc => 
        bounds.contains([inc.coordinates.lat, inc.coordinates.lng])
      ).map(inc => inc.id);
      
      onBoundsChangeRef.current(visible);
    }, 100);

    return () => clearTimeout(timeoutId);
  }, [incidents]);

  return (
    <div className="relative w-full h-full z-30">
      <div id="map" className="w-full h-full" />
      
      {/* Leyenda */}
      <div className="absolute bottom-4 right-4 bg-white rounded-lg shadow-lg p-3" style={{ zIndex: 400 }}>
        <h4 className="text-xs font-semibold mb-2">Prioridad</h4>
        <div className="space-y-1">
          <div className="flex items-center gap-2">
            <div className="w-4 h-4 rounded-md bg-red-500"></div>
            <span className="text-xs">Crítica</span>
          </div>
          <div className="flex items-center gap-2">
            <div className="w-4 h-4 rounded-md bg-orange-500"></div>
            <span className="text-xs">Alta</span>
          </div>
          <div className="flex items-center gap-2">
            <div className="w-4 h-4 rounded-md bg-yellow-500"></div>
            <span className="text-xs">Media</span>
          </div>
          <div className="flex items-center gap-2">
            <div className="w-4 h-4 rounded-md bg-blue-500"></div>
            <span className="text-xs">Baja</span>
          </div>
        </div>
      </div>
    </div>
  );
}

export default HeatMap;
