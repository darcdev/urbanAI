import { useCallback, useEffect, useState } from "react"
import { Incident } from "../types"

export function useIncidents() {
  const [incidents, setIncidents] = useState<Incident[]>([])
  const [selectedCategory, setSelectedCategory] = useState<string>("all")
  const [searchCode, setSearchCode] = useState("")

  const loadIncidents = useCallback(() => {
    const stored = localStorage.getItem("incidents")
    if (stored) {
      const allIncidents: Incident[] = JSON.parse(stored)
      setIncidents(allIncidents)
    }
  }, [])

  useEffect(() => {
    loadIncidents()
  }, [loadIncidents])

  const saveIncident = (incident: Incident) => {
    const stored = localStorage.getItem("incidents")
    const list: Incident[] = stored ? JSON.parse(stored) : []
    list.unshift(incident)
    localStorage.setItem("incidents", JSON.stringify(list))
    setIncidents(list)
  }

  const filteredIncidents = incidents.filter((incident) => {
    const matchCategory =
      selectedCategory === "all" || (incident.category || "Sin categoría") === selectedCategory
    const matchCode = searchCode.trim()
      ? incident.code.toLowerCase().includes(searchCode.trim().toLowerCase())
      : true
    return matchCategory && matchCode
  })

  return {
    incidents,
    filteredIncidents,
    selectedCategory,
    setSelectedCategory,
    searchCode,
    setSearchCode,
    saveIncident,
  }
}


