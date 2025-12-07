import { useEffect, useRef } from "react"

type LeafletMap = {
  setView: (coords: [number, number], zoom?: number) => void
  remove: () => void
}

type LeafletMarker = {
  setLatLng: (coords: [number, number]) => void
}

export function useLeafletMap() {
  const mapRef = useRef<HTMLDivElement>(null)
  const leafletRef = useRef<{
    map?: LeafletMap
    marker?: LeafletMarker
  }>({})

  useEffect(() => {
    const refSnapshot = leafletRef.current
    async function initMap() {
      if (!mapRef.current || refSnapshot.map) return
      // @ts-expect-error leaflet css
      await import("leaflet/dist/leaflet.css").catch(() => {})
      const L = await import("leaflet")
      const map = L.map(mapRef.current).setView([4.710989, -74.07209], 12)
      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: "© OpenStreetMap contributors",
      }).addTo(map)
      const marker = L.marker([4.710989, -74.07209]).addTo(map)
      refSnapshot.map = map
      refSnapshot.marker = marker
    }
    initMap()
    return () => {
      const refSnapshot = leafletRef.current
      if (refSnapshot.map) {
        refSnapshot.map.remove()
      }
      refSnapshot.map = undefined
      refSnapshot.marker = undefined
    }
  }, [])

  const setMarker = (lat: number, lng: number) => {
    if (leafletRef.current.map && leafletRef.current.marker) {
      leafletRef.current.map.setView([lat, lng], 16)
      leafletRef.current.marker.setLatLng([lat, lng])
    }
  }

  return { mapRef, setMarker }
}


