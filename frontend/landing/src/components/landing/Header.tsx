import { Shield, Menu, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useState } from "react";

const Header = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  return (
    <header className="fixed top-0 left-0 right-0 z-[1000] bg-background/80 backdrop-blur-md border-b border-border">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16 md:h-20">
          {/* Logo */}
          <div className="flex items-center gap-2">
            <div className="w-10 h-10 rounded-lg bg-gradient-hero flex items-center justify-center">
              <Shield className="w-5 h-5 text-primary-foreground" />
            </div>
            <span className="font-serif text-xl font-bold text-foreground">
              Urban<span className="text-primary">IA</span>
            </span>
          </div>

          {/* Desktop Navigation */}
          <nav className="hidden md:flex items-center gap-8">
            <a href="#como-funciona" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors">
              Cómo Funciona
            </a>
            <a href="#mapa" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors">
              Mapa de Incidentes
            </a>
            <a href="#beneficios" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors">
              Beneficios
            </a>
          </nav>

          {/* CTA Buttons */}
          <div className="hidden md:flex items-center gap-3">
            <Button variant="ghost" size="sm">
              Iniciar Sesión
            </Button>
            <Button size="sm" className="bg-gradient-hero hover:opacity-90 transition-opacity">
              Solicitar Demo
            </Button>
          </div>

          {/* Mobile Menu Button */}
          <button
            className="md:hidden p-2"
            onClick={() => setIsMenuOpen(!isMenuOpen)}
            aria-label="Toggle menu"
          >
            {isMenuOpen ? (
              <X className="w-6 h-6 text-foreground" />
            ) : (
              <Menu className="w-6 h-6 text-foreground" />
            )}
          </button>
        </div>

        {/* Mobile Menu */}
        {isMenuOpen && (
          <div className="md:hidden py-4 border-t border-border">
            <nav className="flex flex-col gap-4">
              <a href="#como-funciona" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors">
                Cómo Funciona
              </a>
              <a href="#mapa" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors">
                Mapa de Incidentes
              </a>
              <a href="#beneficios" className="text-sm font-medium text-muted-foreground hover:text-foreground transition-colors">
                Beneficios
              </a>
              <div className="flex flex-col gap-2 pt-4 border-t border-border">
                <Button variant="ghost" size="sm" className="justify-start">
                  Iniciar Sesión
                </Button>
                <Button size="sm" className="bg-gradient-hero">
                  Solicitar Demo
                </Button>
              </div>
            </nav>
          </div>
        )}
      </div>
    </header>
  );
};

export default Header;
