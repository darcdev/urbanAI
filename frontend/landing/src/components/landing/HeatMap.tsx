import { useEffect, useRef, useState } from "react";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import "leaflet.heat";
import { Layers, ZoomIn, ZoomOut, AlertTriangle, TrendingUp, MapPin } from "lucide-react";
import { Button } from "@/components/ui/button";

// Sample incident data (latitude, longitude, intensity)
const incidentData: [number, number, number][] = [
  // Centro de la ciudad (alta densidad)
  [19.4326, -99.1332, 0.9],
  [19.4330, -99.1340, 0.8],
  [19.4320, -99.1325, 0.7],
  [19.4335, -99.1350, 0.6],
  [19.4315, -99.1310, 0.8],
  
  // Zona norte
  [19.4450, -99.1400, 0.5],
  [19.4460, -99.1420, 0.6],
  [19.4440, -99.1380, 0.4],
  
  // Zona sur
  [19.4200, -99.1500, 0.7],
  [19.4180, -99.1520, 0.8],
  [19.4210, -99.1480, 0.5],
  [19.4190, -99.1540, 0.6],
  
  // Zona este
  [19.4350, -99.1150, 0.6],
  [19.4360, -99.1130, 0.5],
  [19.4340, -99.1170, 0.4],
  
  // Zona oeste
  [19.4280, -99.1600, 0.7],
  [19.4270, -99.1620, 0.6],
  [19.4290, -99.1580, 0.5],
  
  // Puntos adicionales dispersos
  [19.4400, -99.1250, 0.4],
  [19.4150, -99.1350, 0.5],
  [19.4380, -99.1450, 0.3],
  [19.4250, -99.1200, 0.6],
];

const incidentTypes = [
  { label: "Baches", count: 45, color: "bg-[#EF4444]" },
  { label: "Tuberías rotas", count: 23, color: "bg-[#3B82F6]" },
  { label: "Accidentes", count: 12, color: "bg-[#F59E0B]" },
  { label: "Alumbrado", count: 34, color: "bg-[#10B981]" },
];

const HeatMap = () => {
  const mapContainer = useRef<HTMLDivElement>(null);
  const mapRef = useRef<L.Map | null>(null);
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    if (!mapContainer.current || mapRef.current) return;

    // Initialize map
    mapRef.current = L.map(mapContainer.current, {
      center: [19.4326, -99.1332],
      zoom: 13,
      zoomControl: false,
    });

    // Add tile layer
    L.tileLayer("https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png", {
      attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
      subdomains: "abcd",
      maxZoom: 19,
    }).addTo(mapRef.current);

    // Add heat layer
    const heat = L.heatLayer(incidentData, {
      radius: 35,
      blur: 25,
      maxZoom: 17,
      max: 1.0,
      gradient: {
        0.2: "#3B82F6",
        0.4: "#10B981",
        0.6: "#F59E0B",
        0.8: "#EF4444",
        1.0: "#DC2626",
      },
    });

    heat.addTo(mapRef.current);
    setIsLoaded(true);

    return () => {
      if (mapRef.current) {
        mapRef.current.remove();
        mapRef.current = null;
      }
    };
  }, []);

  const handleZoomIn = () => {
    mapRef.current?.zoomIn();
  };

  const handleZoomOut = () => {
    mapRef.current?.zoomOut();
  };

  return (
    <section id="mapa" className="py-16 md:py-24 bg-secondary/30">
      <div className="container mx-auto px-4">
        {/* Section Header */}
        <div className="max-w-2xl mx-auto text-center mb-12">
          <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-primary/10 text-primary text-sm font-semibold mb-4">
            <MapPin className="w-4 h-4" />
            Visualización en Tiempo Real
          </div>
          <h2 className="text-3xl md:text-4xl font-bold text-foreground mb-4">
            Mapa de Calor de <span className="text-primary">Incidentes</span>
          </h2>
          <p className="text-lg text-muted-foreground">
            Visualiza la concentración de incidentes reportados y toma decisiones basadas en datos.
          </p>
        </div>

        {/* Map Container */}
        <div className="relative rounded-2xl overflow-hidden border-2 border-primary/20 shadow-elevated">
          {/* Map */}
          <div
            ref={mapContainer}
            className="w-full h-[500px] md:h-[600px] bg-muted"
            style={{ zIndex: 1 }}
          />

          {/* Loading Overlay */}
          {!isLoaded && (
            <div className="absolute inset-0 bg-muted flex items-center justify-center z-20">
              <div className="animate-spin w-8 h-8 border-4 border-primary border-t-transparent rounded-full" />
            </div>
          )}

          {/* Zoom Controls */}
          <div className="absolute top-4 right-4 flex flex-col gap-2 z-[999]">
            <Button
              size="icon"
              className="bg-card hover:bg-card/90 text-foreground shadow-elevated border border-border"
              onClick={handleZoomIn}
            >
              <ZoomIn className="w-5 h-5 text-primary" />
            </Button>
            <Button
              size="icon"
              className="bg-card hover:bg-card/90 text-foreground shadow-elevated border border-border"
              onClick={handleZoomOut}
            >
              <ZoomOut className="w-5 h-5 text-primary" />
            </Button>
            <Button
              size="icon"
              className="bg-card hover:bg-card/90 text-foreground shadow-elevated border border-border"
            >
              <Layers className="w-5 h-5 text-primary" />
            </Button>
          </div>

          {/* Legend Card */}
          <div className="absolute bottom-4 left-4 p-5 rounded-xl bg-card border-2 border-primary/20 shadow-elevated z-[999] min-w-[200px]">
            <div className="flex items-center gap-2 mb-4 pb-3 border-b border-border">
              <div className="w-8 h-8 rounded-lg bg-warning/20 flex items-center justify-center">
                <AlertTriangle className="w-5 h-5 text-warning" />
              </div>
              <span className="text-base font-bold text-foreground">Tipos de Incidentes</span>
            </div>
            <div className="space-y-3">
              {incidentTypes.map((type) => (
                <div key={type.label} className="flex items-center justify-between gap-6">
                  <div className="flex items-center gap-3">
                    <div className={`w-4 h-4 rounded-full ${type.color} shadow-sm`} />
                    <span className="text-sm font-medium text-foreground">{type.label}</span>
                  </div>
                  <span className="text-sm font-bold text-primary bg-primary/10 px-2 py-0.5 rounded-full">{type.count}</span>
                </div>
              ))}
            </div>
          </div>

          {/* Stats Card */}
          <div className="absolute top-4 left-4 p-5 rounded-xl bg-card border-2 border-primary/20 shadow-elevated z-[999]">
            <div className="flex items-center gap-3 mb-3">
              <div className="w-10 h-10 rounded-lg bg-primary/20 flex items-center justify-center">
                <MapPin className="w-6 h-6 text-primary" />
              </div>
              <div>
                <div className="text-3xl font-bold text-foreground">114</div>
                <div className="text-sm text-muted-foreground font-medium">Incidentes reportados</div>
              </div>
            </div>
            <div className="flex items-center gap-2 pt-3 border-t border-border">
              <TrendingUp className="w-4 h-4 text-accent" />
              <span className="text-sm font-bold text-accent">+12% esta semana</span>
            </div>
          </div>

          {/* Heat Scale Legend */}
          <div className="absolute bottom-4 right-4 p-4 rounded-xl bg-card border-2 border-primary/20 shadow-elevated z-[999]">
            <span className="text-xs font-bold text-foreground block mb-2">Densidad</span>
            <div className="flex items-center gap-2">
              <span className="text-xs text-muted-foreground">Baja</span>
              <div className="w-24 h-3 rounded-full" style={{
                background: "linear-gradient(to right, #3B82F6, #10B981, #F59E0B, #EF4444, #DC2626)"
              }} />
              <span className="text-xs text-muted-foreground">Alta</span>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeatMap;
