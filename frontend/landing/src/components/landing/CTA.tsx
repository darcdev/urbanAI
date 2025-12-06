import { ArrowRight, Mail, Phone } from "lucide-react";
import { Button } from "@/components/ui/button";

const CTA = () => {
  return (
    <section className="py-16 md:py-24">
      <div className="container mx-auto px-4">
        <div className="relative rounded-2xl bg-gradient-hero p-8 md:p-12 lg:p-16 overflow-hidden">
          {/* Background Pattern */}
          <div className="absolute inset-0 opacity-10">
            <div className="absolute inset-0" style={{
              backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='1'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
            }} />
          </div>

          <div className="relative max-w-3xl mx-auto text-center">
            <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold text-primary-foreground mb-4">
              Transforma tu ciudad hoy
            </h2>
            <p className="text-lg md:text-xl text-primary-foreground/80 mb-8 max-w-2xl mx-auto">
              Únete a las entidades gubernamentales que ya están mejorando la calidad de vida de sus ciudadanos con UrbanIA.
            </p>

            {/* CTA Buttons */}
            <div className="flex flex-col sm:flex-row items-center justify-center gap-4 mb-8">
              <Button
                size="lg"
                variant="secondary"
                className="gap-2 text-base px-8 bg-primary-foreground text-primary hover:bg-primary-foreground/90"
              >
                Solicitar Demo Gratuita
                <ArrowRight className="w-4 h-4" />
              </Button>
              <Button
                size="lg"
                variant="outline"
                className="gap-2 text-base px-8 border-primary-foreground/30 text-primary-foreground bg-primary-foreground/10 hover:bg-primary-foreground/10"
              >
                Ver Casos de Éxito
              </Button>
            </div>

            {/* Contact Info */}
            <div className="flex flex-col sm:flex-row items-center justify-center gap-6 text-primary-foreground/70">
              <a href="mailto:contacto@urbania.gov" className="flex items-center gap-2 hover:text-primary-foreground transition-colors">
                <Mail className="w-4 h-4" />
                <span className="text-sm">contacto@urbania.gov</span>
              </a>
              <a href="tel:+525555555555" className="flex items-center gap-2 hover:text-primary-foreground transition-colors">
                <Phone className="w-4 h-4" />
                <span className="text-sm">+52 55 5555 5555</span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default CTA;
