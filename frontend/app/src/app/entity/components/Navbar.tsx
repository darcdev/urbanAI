"use client";

import { LogOut } from "lucide-react";
import { useRouter } from "next/navigation";

export function Navbar() {
  const router = useRouter();

  const handleLogout = () => {
    router.push("/login");
  };

  return (
    <header className="flex h-16 items-center justify-between border-b border-border bg-card px-4 md:px-6 z-10">
      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
            <span className="text-sm font-bold text-primary-foreground">U</span>
          </div>
          <span className="text-lg font-semibold text-foreground">UrbanAI</span>
          <span className="text-sm text-muted-foreground ml-2">Mapa de Incidentes</span>
        </div>
      </div>

      <div className="flex items-center gap-2 md:gap-3">
        <button
          onClick={handleLogout}
          className="flex items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium text-foreground hover:bg-accent"
        >
          <LogOut className="h-4 w-4" />
          <span className="hidden sm:inline">Salir</span>
        </button>
      </div>
    </header>
  );
}
