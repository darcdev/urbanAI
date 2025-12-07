"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { 
  Home,
  ChevronLeft,
  X
} from "lucide-react";

const menuItems = [
  {
    title: "Mapa de Incidentes",
    icon: Home,
    href: "/entity",
  },
];

interface SidebarProps {
  isOpen: boolean;
  isCollapsed: boolean;
  onClose: () => void;
  onToggleCollapse: () => void;
}

export function Sidebar({ isOpen, isCollapsed, onClose, onToggleCollapse }: SidebarProps) {
  const pathname = usePathname();

  return (
    <>
      {/* Overlay para móvil */}
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-black/50 backdrop-blur-sm transition-opacity lg:hidden"
          onClick={onClose}
        />
      )}

      {/* Sidebar */}
      <aside
        className={`bg-white fixed lg:relative inset-y-0 left-0 z-50 flex flex-col border-r border-border bg-sidebar-background transition-all duration-300 ease-in-out ${
          isCollapsed ? "lg:w-16" : "lg:w-64"
        } ${
          isOpen ? "translate-x-0" : "-translate-x-full lg:translate-x-0"
        } w-64`}
      >
        {/* Logo Section */}
        <div className="flex h-16 items-center justify-between border-b border-sidebar-border px-4">
          {!isCollapsed && (
            <div className="flex items-center gap-2">
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
                <span className="text-sm font-bold text-primary-foreground">U</span>
              </div>
              <span className="text-lg font-semibold text-sidebar-foreground">
                UrbanAI
              </span>
            </div>
          )}
          {/* Botón de cerrar en móvil, colapsar en desktop */}
          <button
            onClick={() => {
              if (typeof window !== 'undefined' && window.innerWidth < 1024) {
                onClose();
              } else {
                onToggleCollapse();
              }
            }}
            className="flex h-8 w-8 items-center justify-center rounded-md hover:bg-sidebar-accent"
          >
            <X className="h-5 w-5 text-sidebar-foreground lg:hidden" />
            <ChevronLeft className={`hidden lg:block h-5 w-5 text-sidebar-foreground transition-transform ${isCollapsed ? 'rotate-180' : ''}`} />
          </button>
        </div>

        {/* Navigation */}
        <nav className="flex-1 space-y-1 p-3">
          {menuItems.map((item) => {
            const Icon = item.icon;
            const isActive = pathname === item.href;

            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={() => {
                  if (typeof window !== 'undefined' && window.innerWidth < 1024) {
                    onClose();
                  }
                }}
                className={`flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors ${
                  isActive
                    ? "bg-primary text-white"
                    : "text-sidebar-foreground hover:bg-sidebar-accent/50"
                }`}
                title={isCollapsed ? item.title : undefined}
              >
                <Icon className="h-5 w-5 shrink-0" />
                {!isCollapsed && <span>{item.title}</span>}
              </Link>
            );
          })}
        </nav>

        {/* User Section */}
        <div className="border-t border-sidebar-border p-3">
          <div
            className={`flex items-center gap-3 rounded-lg px-3 py-2.5 ${
              isCollapsed ? "justify-center" : ""
            }`}
          >
            <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-primary text-primary-foreground">
              <span className="text-xs font-semibold">EN</span>
            </div>
            {!isCollapsed && (
              <div className="flex flex-col">
                <span className="text-sm font-medium text-sidebar-foreground">
                  Entidad
                </span>
                <span className="text-xs text-sidebar-foreground/60">
                  entidad@urbanai.com
                </span>
              </div>
            )}
          </div>
        </div>
      </aside>
    </>
  );
}
