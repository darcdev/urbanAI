import { Camera, MapPin, BarChart3, ArrowRight } from "lucide-react";
import { Button } from "@/components/ui/button";

const Hero = () => {
  return (
    <section className="relative pt-24 md:pt-32 pb-16 md:pb-24 overflow-hidden">
      {/* Background Pattern */}
      <div className="absolute inset-0 opacity-[0.03]">
        <div className="absolute inset-0" style={{
          backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23000000' fill-opacity='1'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
        }} />
      </div>

      <div className="container mx-auto px-4 relative">
        <div className="max-w-4xl mx-auto text-center">
          {/* Badge */}
          <div className="inline-flex items-center gap-2 px-4 py-2 rounded-md bg-primary/10 text-primary text-sm font-semibold mb-6 animate-fade-in">
            <span className="w-2.5 h-2.5 rounded-md bg-accent animate-pulse-soft" />
            Plataforma de Gestión Urbana Inteligente
          </div>

          {/* Headline */}
          <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold text-foreground leading-tight mb-6 animate-fade-in" style={{ animationDelay: "0.1s" }}>
            Reporta incidentes urbanos.{" "}
            <span className="text-gradient">Protege tu ciudad.</span>
          </h1>

          {/* Subheadline */}
          <p className="text-lg md:text-xl text-muted-foreground max-w-2xl mx-auto mb-8 animate-fade-in" style={{ animationDelay: "0.2s" }}>
            Sistema de reporte ciudadano con geolocalización automática y visualización en tiempo real para entidades gubernamentales.
          </p>

          {/* CTA Buttons */}
          <div className="flex flex-col sm:flex-row items-center justify-center gap-4 mb-16 animate-fade-in" style={{ animationDelay: "0.3s" }}>
            <Button size="lg" className="bg-gradient-hero hover:opacity-90 transition-opacity gap-2 text-base px-8 shadow-lg">
              Solicitar Demo
              <ArrowRight className="w-5 h-5" />
            </Button>
            <Button size="lg" variant="outline" className="gap-2 text-base px-8 border-2 border-primary/30 text-primary hover:bg-primary/5">
              Ver Documentación
            </Button>
          </div>

          {/* Feature Pills */}
          <div className="flex flex-wrap items-center justify-center gap-4 animate-fade-in" style={{ animationDelay: "0.4s" }}>
            <div className="flex items-center gap-3 px-5 py-3 rounded-xl bg-card shadow-soft border-2 border-primary/20 hover:border-primary/40 transition-colors">
              <div className="w-10 h-10 rounded-lg bg-primary/15 flex items-center justify-center">
                <Camera className="w-5 h-5 text-primary" strokeWidth={2.5} />
              </div>
              <span className="text-sm font-semibold text-foreground">Captura Instantánea</span>
            </div>
            <div className="flex items-center gap-3 px-5 py-3 rounded-xl bg-card shadow-soft border-2 border-accent/20 hover:border-accent/40 transition-colors">
              <div className="w-10 h-10 rounded-lg bg-accent/15 flex items-center justify-center">
                <MapPin className="w-5 h-5 text-accent" strokeWidth={2.5} />
              </div>
              <span className="text-sm font-semibold text-foreground">Geolocalización GPS</span>
            </div>
            <div className="flex items-center gap-3 px-5 py-3 rounded-xl bg-card shadow-soft border-2 border-info/20 hover:border-info/40 transition-colors">
              <div className="w-10 h-10 rounded-lg bg-info/15 flex items-center justify-center">
                <BarChart3 className="w-5 h-5 text-info" strokeWidth={2.5} />
              </div>
              <span className="text-sm font-semibold text-foreground">Mapa de Calor</span>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Hero;
