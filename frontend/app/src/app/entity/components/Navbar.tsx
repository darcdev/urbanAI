"use client";

import { LogOut } from "lucide-react";
import { useRouter } from "next/navigation";
import Image from "next/image";

export function Navbar() {
  const router = useRouter();

  const handleLogout = () => {
    router.push("/login");
  };

  return (
    <header className="flex h-16 items-center justify-between border-b border-border bg-current px-4 md:px-6 z-10">
      <div className="flex items-center gap-3">
        <div className="flex items-center gap-2">
          <div className="flex items-center gap-2">
            <Image
              src="/images/Logo-liviano.png"
              alt="UrbanAI"
              width={120}
              height={40}
              className="object-contain"
            />
          </div>
          <span className="text-sm ml-2 text-white">Mapa de Incidentes</span>
        </div>
      </div>

      <div className="flex items-center gap-2 md:gap-3">
        <button
          onClick={handleLogout}
          className="flex items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium text-foreground hover:bg-gray-200 bg-white cursor-pointer transition-all"
        >
          <LogOut className="h-4 w-4" />
          <span className="hidden sm:inline">Salir</span>
        </button>
      </div>
    </header>
  );
}
