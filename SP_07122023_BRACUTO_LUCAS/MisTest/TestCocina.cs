using Entidades.Exceptions;
using Entidades.Files;
using Entidades.Modelos;

namespace MisTest
{
    [TestClass]
    public class TestCocina
    {
        [TestMethod]
        [ExpectedException(typeof(FileManagerException))]
        public void AlGuardarUnArchivo_ConNombreInvalido_TengoUnaExcepcion()
        {
            //arrange
            string dataPrueba = "Data de prueba";
            string nombreArchivo = "archivo/:.txt";
            //act
            FileManager.Guardar(dataPrueba, nombreArchivo, true);
            //assert
        }

        [TestMethod]

        public void AlInstanciarUnCocinero_SeEspera_PedidosCero()
        {
            //arrange
            int resultado = 0;
            int pedidos;
            Cocinero<Hamburguesa> hamburguesero = new Cocinero<Hamburguesa>("Pepe");
            //act
            pedidos = hamburguesero.CantPedidosFinalizados;

            //assert
            Assert.AreEqual(pedidos, resultado);
        }
    }
}