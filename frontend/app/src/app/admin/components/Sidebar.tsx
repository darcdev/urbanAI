"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import Image from "next/image";
import { useState, useEffect } from "react";
import { 
  Home,
  ChevronLeft,
  X
} from "lucide-react";
import { authService } from "@/lib/api/auth.service";

const menuItems = [
  {
    title: "Usuarios",
    icon: Home,
    href: "/admin",
  },
  {
    title: "Organizaciones",
    icon: Home,
    href: "/admin/organizations",
  }
];

interface SidebarProps {
  isOpen: boolean;
  isCollapsed: boolean;
  onClose: () => void;
  onToggleCollapse: () => void;
}

export function Sidebar({ isOpen, isCollapsed, onClose, onToggleCollapse }: SidebarProps) {
  const pathname = usePathname();
  const [user, setUser] = useState<{ email?: string } | null>(null);

  useEffect(() => {
    // Obtener información del usuario desde localStorage
    const fetchUser = () => {
      const userData = authService.getUser();
      setUser(userData);
    };
    fetchUser();
  }, []);

  // Generar iniciales del email
  const getInitials = () => {
    if (!user?.email) return "U";
    return user.email.substring(0, 2).toUpperCase();
  };

  return (
    <>
      {isOpen && (
        <div
          className="fixed inset-0 z-40 bg-black/50 backdrop-blur-sm transition-opacity lg:hidden"
          onClick={onClose}
        />
      )}

      <aside
        className={`bg-current fixed lg:relative inset-y-0 left-0 z-50 flex flex-col border-r border-border bg-sidebar-background transition-all duration-300 ease-in-out ${
          isCollapsed ? "lg:w-16" : "lg:w-64"
        } ${
          isOpen ? "translate-x-0" : "-translate-x-full lg:translate-x-0"
        } w-64`}
      >
        <div className="flex h-16 items-center justify-between border-b border-sidebar-border px-4">
          {!isCollapsed && (
            <div className="flex items-center gap-2">
              <Image
                src="/images/Logo-liviano.png"
                alt="UrbanAI"
                width={120}
                height={40}
                className="object-contain"
              />
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
            className="flex h-8 w-8 items-center justify-center rounded-md hover:bg-sidebar-accent bg-white cursor-pointer"
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
                    : "text-sidebar-foreground hover:bg-sidebar-accent/50 text-white"
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
            <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md bg-primary text-primary-foreground">
              <span className="text-xs font-semibold">{getInitials()}</span>
            </div>
            {!isCollapsed && (
              <div className="flex flex-col text-white">
                <span className="text-sm font-medium text-sidebar-foreground">
                  {user?.email || "usuario@urbanai.com"}
                </span>
              </div>
            )}
          </div>
        </div>
      </aside>
    </>
  );
}