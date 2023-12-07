using Entidades.Enumerados;


namespace Entidades.MetodosDeExtension
{
    public static class IngredientesExtension
    {
        public static double CalcularCostoIngredientes(this List<EIngrediente> ingredientes, int costoInicial)
        {
            double costo = costoInicial;
            if (ingredientes is not null)
            {
                foreach (var e in ingredientes)
                {
                    costo += (costoInicial * (double)e) / 100;
                }
            }
            return costo;
        }

        public static List<EIngrediente> IngredientesAleatorios(this Random rand)
        {
            List<EIngrediente> ingredientes = new List<EIngrediente>()
            {
                EIngrediente.QUESO,
                EIngrediente.PANCETA,
                EIngrediente.ADHERESO,
                EIngrediente.HUEVO,
                EIngrediente.JAMON,
            };
            return ingredientes.Take(rand.Next(1, 6)).ToList();
        }
    }
}
