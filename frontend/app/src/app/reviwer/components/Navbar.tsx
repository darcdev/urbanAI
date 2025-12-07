"use client";

import { LogOut, Menu } from "lucide-react";
import { useRouter } from "next/navigation";

interface NavbarProps {
  onMenuClick: () => void;
}

export function Navbar({ onMenuClick }: NavbarProps) {
  const router = useRouter();

  const handleLogout = () => {
    router.push("/login");
  };

  return (
    <header className="flex h-16 items-center justify-between border-b border-border bg-card px-4 md:px-6">
      <div className="flex items-center gap-3">
        <button
          onClick={onMenuClick}
          className="flex h-10 w-10 items-center justify-center rounded-lg hover:bg-accent lg:hidden"
        >
          <Menu className="h-5 w-5 text-foreground" />
        </button>

        <div className="flex items-center gap-2 lg:hidden">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
            <span className="text-sm font-bold text-primary-foreground">U</span>
          </div>
          <span className="text-lg font-semibold text-foreground">UrbanAI</span>
        </div>
      </div>

      <div className="flex items-center gap-2 md:gap-3">

        <div className="hidden h-6 w-px bg-border md:block" />

        <button
          onClick={handleLogout}
          className="flex items-center gap-2 rounded-lg px-3 py-2 text-sm font-medium text-foreground hover:bg-gray-200 cursor-pointer transition-all"
        >
          <LogOut className="h-4 w-4" />
          <span className="hidden sm:inline">Salir</span>
        </button>
      </div>
    </header>
  );
}
