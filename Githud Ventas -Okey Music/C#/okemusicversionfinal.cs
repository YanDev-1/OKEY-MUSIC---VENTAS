using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

class Program
{
    // ESTRUCTURA DE PRODUCTO
    class Producto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
        public string Categoria { get; set; }
        public int PosicionOriginal { get; set; }

        public Producto(string codigo, string nombre, string marca, double precio, string categoria, int orden)
        {
            Codigo = codigo;
            Nombre = nombre;
            Marca = marca;
            Precio = precio;
            Stock = 0;
            Categoria = categoria;
            PosicionOriginal = orden;
        }
    }

    // REGISTRO DE HISTORIALES
    class RegistroStock
    {
        public DateTime FechaHora { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int CantidadIngresada { get; set; }
    }

    // NUEVA ESTRUCTURA DE GASTO
    class Gasto
    {
        public string Empleado { get; set; }
        public double Monto { get; set; }
        public string Descripcion { get; set; }
        public string Proposito { get; set; }
        public DateTime FechaHora { get; set; }
    }

    // LISTAS PRINCIPALES
    static List<Producto> productos = new List<Producto>();

    // ARREGLOS PARALELOS DE VENTAS
    const int MAX_REGISTROS_VENTA = 1000;
    static string[] codigosVenta = new string[MAX_REGISTROS_VENTA];
    static DateTime[] fechasVenta = new DateTime[MAX_REGISTROS_VENTA];
    static string[] codigosProductoVenta = new string[MAX_REGISTROS_VENTA];
    static string[] nombresProductoVenta = new string[MAX_REGISTROS_VENTA];
    static int[] cantidadesVenta = new int[MAX_REGISTROS_VENTA];
    static double[] subtotalesVenta = new double[MAX_REGISTROS_VENTA];
    static double[] descuentosVenta = new double[MAX_REGISTROS_VENTA];
    static int totalRegistrosVenta = 0;

    static List<RegistroStock> historialStock = new List<RegistroStock>();
    static List<Gasto> listaGastos = new List<Gasto>(); // Nueva lista para Gastos
    static int contadorVentas = 0;

    static readonly string archivoProductos = "productos.txt";
    static readonly string archivoVentas = "ventas.txt";
    static readonly string archivoStock = "historial_stock.txt";
    static readonly string archivoGastos = "gastos.txt"; // Nuevo archivo para Gastos

    static void Main(string[] args)
    {
        CargarProductosIniciales();
        CargarDatosGuardados();
        int opcion;

        do
        {
            MostrarMenu();
            while (!int.TryParse(Console.ReadLine(), out opcion) || opcion < 1 || opcion > 14)
            {
                Console.WriteLine("Error: Ingrese un número válido (1-14)");
                Console.Write("Ingrese opción: ");
            }

            switch (opcion)
            {
                case 1: RegistrarStock(); break;
                case 2: MostrarProductosPorCategoria(); break;
                case 3: VenderProductos(); break;
                case 4: BuscarVentaPorCodigo(); break;
                case 5: BuscarProductoExacto(); break;
                case 6: BuscarProductoParcial(); break;
                case 7: ModificarProducto(); break;
                case 8: EliminarProductos(); break;
                case 9: OrdenarProductos(); break;
                case 10: MostrarGanancias(); break;
                case 11: MostrarHistorialStock(); break;
                case 12: RegistrarGasto(); break;
                case 13: ResumenDelDia(); break;
                case 14:
                    Console.WriteLine("Saliendo del sistema...");
                    break;
                default:
                    Console.WriteLine("Opción no válida. Elija un número del 1 al 14.");
                    break;
            }

            if (opcion != 14)
            {
                if (!Console.IsInputRedirected)
                {
                    Console.WriteLine("\nPresione cualquier tecla para continuar...");
                    try { Console.ReadKey(true); } catch { Console.ReadLine(); }
                    Console.Clear();
                }
            }

        } while (opcion != 14);
    }

    static void MostrarMenu()
    {
        Console.WriteLine("===========================OKEY MUSIC=================================");
        Console.WriteLine("=================== SISTEMA DE INVENTARIO Y VENTAS ===================");
        Console.WriteLine("1. Registrar stock");
        Console.WriteLine("2. Mostrar productos (por categoría)");
        Console.WriteLine("3. Vender productos");
        Console.WriteLine("4. Buscar venta por código");
        Console.WriteLine("5. Buscar producto (exacto)");
        Console.WriteLine("6. Buscar producto (parcial)");
        Console.WriteLine("7. Modificar producto (Nombre y Precio)");
        Console.WriteLine("8. Eliminar producto(s)");
        Console.WriteLine("9. Ordenar productos");
        Console.WriteLine("10. Ver ganancias (Historial de ventas con fecha y hora)");
        Console.WriteLine("11. Ver historial de ingreso de stock");
        Console.WriteLine("12. Registrar gasto");
        Console.WriteLine("13. Resumen del día (Ventas y Gastos)");
        Console.WriteLine("14. Salir");
        Console.WriteLine("=====================================================================");
        Console.Write("Elija una opción: ");
    }

    // 1. REGISTRAR STOCK
    static void RegistrarStock()
    {
        Console.WriteLine("\n--- REGISTRAR STOCK ---");
        Console.Write("Ingrese el código único del producto: ");
        string codigo = Console.ReadLine().Trim().ToUpper();

        Producto prod = productos.Find(p => p.Codigo.ToUpper() == codigo);
        if (prod == null)
        {
            Console.WriteLine("Error: Código de producto no encontrado.");
            return;
        }

        Console.WriteLine($"Producto seleccionado: {prod.Nombre} | Stock actual: {prod.Stock}");
        Console.Write("Ingrese la cantidad de stock a agregar: ");
        int cantidad;
        while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0)
        {
            Console.Write("Error. Ingrese una cantidad entera positiva: ");
        }

        prod.Stock += cantidad;
        GuardarProductos();

        historialStock.Add(new RegistroStock
        {
            FechaHora = DateTime.Now,
            Codigo = prod.Codigo,
            Nombre = prod.Nombre,
            CantidadIngresada = cantidad
        });
        GuardarHistorialStock();

        Console.WriteLine($"¡Stock actualizado con éxito! Nuevo stock de {prod.Nombre}: {prod.Stock} unidades.");
    }

    // 2. MOSTRAR PRODUCTOS POR CATEGORÍAS
    static void MostrarProductosPorCategoria()
    {
        if (productos.Count == 0)
        {
            Console.WriteLine("\nNo hay productos registrados en el sistema.");
            return;
        }

        string[] categorias = {
            "Guitarras (Nylon y Metal)",
            "Teclados",
            "Flautas, Melódicas y Armónicas",
            "Vientos Metal (Trompetas, Cornetas y Trombones)"
        };

        foreach (var cat in categorias)
        {
            Console.WriteLine($"\n================== {cat.ToUpper()} ==================");
            Console.WriteLine(string.Format("{0,-10} | {1,-42} | {2,-15} | {3,-10} | {4,-6}",
                "CÓDIGO", "NOMBRE DEL PRODUCTO", "MARCA", "PRECIO", "STOCK"));
            Console.WriteLine(new string('-', 92));

            bool hayEnCat = false;
            foreach (var p in productos)
            {
                if (p.Categoria == cat)
                {
                    hayEnCat = true;
                    Console.WriteLine(string.Format("{0,-10} | {1,-42} | {2,-15} | S/.{3,-7:0.00} | {4,-6}",
                        p.Codigo,
                        p.Nombre.Length > 42 ? p.Nombre.Substring(0, 39) + "..." : p.Nombre,
                        p.Marca,
                        p.Precio,
                        p.Stock));
                }
            }

            if (!hayEnCat)
            {
                Console.WriteLine("No hay productos en esta categoría.");
            }
        }
    }

    // 3. VENDER PRODUCTOS
    static void VenderProductos()
    {
        Console.WriteLine("\n--- REALIZAR VENTA ---");
        Console.Write("¿Cuántos tipos de productos diferentes desea vender en esta transacción?: ");

        int totalItems;
        while (!int.TryParse(Console.ReadLine(), out totalItems) || totalItems <= 0)
        {
            Console.Write("Ingrese un número válido mayor a 0: ");
        }

        List<string> codigosTemp = new List<string>();
        List<DateTime> fechasTemp = new List<DateTime>();
        List<string> nombresTemp = new List<string>();
        List<int> cantidadesTemp = new List<int>();
        List<double> subtotalesTemp = new List<double>();
        List<double> descuentosTemp = new List<double>();

        for (int i = 0; i < totalItems; i++)
        {
            Console.WriteLine($"\n[Ítem {i + 1} de {totalItems}]");
            Console.Write("Ingrese el código único del producto: ");
            string codigo = Console.ReadLine().Trim().ToUpper();

            Producto prod = productos.Find(p => p.Codigo.ToUpper() == codigo);
            if (prod == null)
            {
                Console.WriteLine("Código no encontrado. Se omitirá este producto.");
                continue;
            }

            Console.WriteLine($"Producto: {prod.Nombre} | Precio: S/.{prod.Precio:0.00} | Stock: {prod.Stock}");
            Console.Write("Ingrese cantidad a vender: ");

            int cantidad;
            while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0)
            {
                Console.Write("Ingrese una cantidad válida: ");
            }

            if (prod.Stock >= cantidad)
            {
                Console.Write($"Ingrese el precio final por unidad (Enter = S/.{prod.Precio:0.00}, sin descuento): ");
                string entradaPrecioFinal = Console.ReadLine().Trim();

                double precioFinalUnitario = prod.Precio;

                if (!string.IsNullOrWhiteSpace(entradaPrecioFinal))
                {
                    while (!double.TryParse(entradaPrecioFinal, out precioFinalUnitario) ||
                           precioFinalUnitario <= 0 || precioFinalUnitario > prod.Precio)
                    {
                        Console.Write($"Precio no válido. Debe ser mayor a 0 y no superar S/.{prod.Precio:0.00}: ");
                        entradaPrecioFinal = Console.ReadLine().Trim();
                    }
                }

                double subtotal = prod.Precio * cantidad;
                double descuento = (prod.Precio - precioFinalUnitario) * cantidad;
                double totalLinea = precioFinalUnitario * cantidad;

                prod.Stock -= cantidad;

                codigosTemp.Add(prod.Codigo);
                fechasTemp.Add(DateTime.Now);
                nombresTemp.Add(prod.Nombre);
                cantidadesTemp.Add(cantidad);
                subtotalesTemp.Add(subtotal);
                descuentosTemp.Add(descuento);

                Console.WriteLine($"Agregado: {cantidad}x {prod.Nombre}");
                Console.WriteLine($"Precio original: S/.{prod.Precio:0.00} por unidad");
                Console.WriteLine($"Descuento: S/.{descuento:0.00}");
                Console.WriteLine($"Total del producto: S/.{totalLinea:0.00}");
            }
            else
            {
                Console.WriteLine($"Stock insuficiente. Solo quedan {prod.Stock} unidades disponibles.");
            }
        }

        if (codigosTemp.Count == 0)
        {
            Console.WriteLine("\nNo se concretó la venta de ningún producto.");
            return;
        }

        if (totalRegistrosVenta + codigosTemp.Count > MAX_REGISTROS_VENTA)
        {
            for (int i = 0; i < codigosTemp.Count; i++)
            {
                Producto producto = productos.Find(p => p.Codigo == codigosTemp[i]);
                if (producto != null)
                {
                    producto.Stock += cantidadesTemp[i];
                }
            }

            Console.WriteLine("No hay espacio disponible para registrar esta venta.");
            return;
        }

        contadorVentas++;
        string codigoVenta = $"VTA-{contadorVentas:0000}";

        for (int i = 0; i < codigosTemp.Count; i++)
        {
            codigosVenta[totalRegistrosVenta] = codigoVenta;
            fechasVenta[totalRegistrosVenta] = fechasTemp[i];
            codigosProductoVenta[totalRegistrosVenta] = codigosTemp[i];
            nombresProductoVenta[totalRegistrosVenta] = nombresTemp[i];
            cantidadesVenta[totalRegistrosVenta] = cantidadesTemp[i];
            subtotalesVenta[totalRegistrosVenta] = subtotalesTemp[i];
            descuentosVenta[totalRegistrosVenta] = descuentosTemp[i];
            totalRegistrosVenta++;
        }

        GuardarProductos();
        GuardarVentas();

        double subtotalGeneral = 0;
        double descuentoGeneral = 0;

        for (int i = 0; i < subtotalesTemp.Count; i++)
        {
            subtotalGeneral += subtotalesTemp[i];
            descuentoGeneral += descuentosTemp[i];
        }

        double totalVenta = subtotalGeneral - descuentoGeneral;

        Console.WriteLine($"\n=== VENTA {codigoVenta} COMPLETADA ===");
        Console.WriteLine($"Subtotal: S/.{subtotalGeneral:0.00}");
        Console.WriteLine($"Descuento total: -S/.{descuentoGeneral:0.00}");
        Console.WriteLine($"Total a pagar: S/.{totalVenta:0.00}");
    }

    // 4. BUSCAR VENTA POR CÓDIGO
    static void BuscarVentaPorCodigo()
    {
        Console.WriteLine("\n--- CONSULTA DE VENTA ---");
        Console.Write("Ingrese el código de venta (ejemplo VTA-0001): ");
        string codigoBuscado = Console.ReadLine().Trim().ToUpper();

        bool encontrada = false;
        double subtotalGeneral = 0;
        double descuentoGeneral = 0;

        for (int i = 0; i < totalRegistrosVenta; i++)
        {
            if (codigosVenta[i] == codigoBuscado)
            {
                if (!encontrada)
                {
                    encontrada = true;

                    Console.WriteLine($"\nCódigo de venta: {codigosVenta[i]}");
                    Console.WriteLine($"Fecha: {fechasVenta[i]:yyyy-MM-dd}");
                    Console.WriteLine($"Hora: {fechasVenta[i]:HH:mm:ss}");
                    Console.WriteLine("-----------------------------------------------");
                }

                double precioOriginalUnitario = subtotalesVenta[i] / cantidadesVenta[i];
                double descuentoUnitario = descuentosVenta[i] / cantidadesVenta[i];
                double precioFinalUnitario = precioOriginalUnitario - descuentoUnitario;
                double totalLinea = subtotalesVenta[i] - descuentosVenta[i];

                Console.WriteLine($"[{codigosProductoVenta[i]}] {nombresProductoVenta[i]}");
                Console.WriteLine($"  Cantidad: {cantidadesVenta[i]}");
                Console.WriteLine($"  Precio original: S/.{precioOriginalUnitario:0.00}");
                Console.WriteLine($"  Precio final acordado: S/.{precioFinalUnitario:0.00}");
                Console.WriteLine($"  Descuento: S/.{descuentoUnitario:0.00} por unidad | S/.{descuentosVenta[i]:0.00} total");
                Console.WriteLine($"  Total del producto: S/.{totalLinea:0.00}");

                subtotalGeneral += subtotalesVenta[i];
                descuentoGeneral += descuentosVenta[i];
            }
        }

        if (!encontrada)
        {
            Console.WriteLine("No se encontró ninguna venta con ese código.");
            return;
        }

        double total = subtotalGeneral - descuentoGeneral;

        Console.WriteLine("-----------------------------------------------");
        Console.WriteLine($"Subtotal: S/.{subtotalGeneral:0.00}");
        Console.WriteLine($"Descuento total: -S/.{descuentoGeneral:0.00}");
        Console.WriteLine($"Total: S/.{total:0.00}");
    }

    // 5. BUSCAR PRODUCTO EXACTO
    static void BuscarProductoExacto()
    {
        Console.WriteLine("\n--- BÚSQUEDA EXACTA ---");
        Console.Write("Ingrese el nombre exacto del producto: ");
        string nombre = Console.ReadLine().Trim();

        Producto prod = productos.Find(p => string.Equals(p.Nombre.Trim(), nombre, StringComparison.OrdinalIgnoreCase));

        if (prod != null)
        {
            Console.WriteLine("\nProducto Encontrado:");
            Console.WriteLine($"• [Código]: {prod.Codigo}");
            Console.WriteLine($"• [Nombre]: {prod.Nombre}");
            Console.WriteLine($"• [Marca]:  {prod.Marca}");
            Console.WriteLine($"• [Precio]: S/.{prod.Precio:0.00}");
            Console.WriteLine($"• [Stock]:  {prod.Stock} unidades");
        }
        else
        {
            Console.WriteLine("No se encontró ningún producto con ese nombre exacto.");
        }
    }

    // 6. BUSCAR PRODUCTO PARCIAL
    static void BuscarProductoParcial()
    {
        Console.WriteLine("\n--- BÚSQUEDA PARCIAL ---");
        Console.Write("Ingrese término o texto a buscar: ");
        string criterio = Console.ReadLine().Trim().ToLower();

        var resultados = productos.FindAll(p => p.Nombre.ToLower().Contains(criterio) || p.Marca.ToLower().Contains(criterio));

        if (resultados.Count > 0)
        {
            Console.WriteLine($"\nSe encontraron {resultados.Count} coincidencias:\n");
            foreach (var prod in resultados)
            {
                Console.WriteLine($"[{prod.Codigo}] | {prod.Nombre} | {prod.Marca} | S/.{prod.Precio:0.00} | Stock: {prod.Stock}");
            }
        }
        else
        {
            Console.WriteLine("No se encontraron coincidencias.");
        }
    }

    // 7. MODIFICAR PRODUCTO
    static void ModificarProducto()
    {
        Console.WriteLine("\n--- MODIFICAR PRODUCTO ---");
        Console.Write("Ingrese el código único del producto a modificar: ");
        string codigo = Console.ReadLine().Trim().ToUpper();

        Producto prod = productos.Find(p => p.Codigo.ToUpper() == codigo);
        if (prod == null)
        {
            Console.WriteLine("Producto no encontrado.");
            return;
        }

        Console.WriteLine($"\nDatos actuales: {prod.Nombre} | S/.{prod.Precio:0.00}");

        Console.Write("Ingrese nuevo nombre (o presione Enter para conservar el actual): ");
        string nuevoNombre = Console.ReadLine().Trim();
        if (!string.IsNullOrWhiteSpace(nuevoNombre))
        {
            prod.Nombre = nuevoNombre;
        }

        Console.Write("Ingrese nuevo precio (o 0 para conservar el actual): ");
        double nuevoPrecio;
        if (double.TryParse(Console.ReadLine(), out nuevoPrecio) && nuevoPrecio > 0)
        {
            prod.Precio = nuevoPrecio;
        }

        GuardarProductos();
        Console.WriteLine("¡Producto modificado con éxito!");
    }

    // 8. ELIMINAR PRODUCTOS
    static void EliminarProductos()
    {
        Console.WriteLine("\n--- ELIMINAR PRODUCTOS ---");
        Console.Write("¿Cuántos productos desea eliminar?: ");
        int cantidad;
        while (!int.TryParse(Console.ReadLine(), out cantidad) || cantidad <= 0)
        {
            Console.Write("Ingrese una cantidad válida mayor a 0: ");
        }

        for (int i = 0; i < cantidad; i++)
        {
            Console.WriteLine($"\n[Eliminación {i + 1} de {cantidad}]");
            Console.Write("Ingrese el código único del producto a eliminar: ");
            string codigo = Console.ReadLine().Trim().ToUpper();

            Producto prod = productos.Find(p => p.Codigo.ToUpper() == codigo);
            if (prod == null)
            {
                Console.WriteLine("Producto no encontrado.");
                continue;
            }

            Console.Write($"¿Confirma la eliminación de '{prod.Nombre}' ({prod.Codigo})? (s/n): ");
            string confirm = Console.ReadLine().Trim().ToLower();
            if (confirm == "s" || confirm == "si")
            {
                productos.Remove(prod);
                GuardarProductos();
                Console.WriteLine("Producto eliminado.");
            }
            else
            {
                Console.WriteLine("Eliminación cancelada para este ítem.");
            }
        }
    }

    // 9. ORDENAR PRODUCTOS
    static void OrdenarProductos()
    {
        Console.WriteLine("\n--- CRITERIOS DE ORDENAMIENTO ---");
        Console.WriteLine("1. Nombre (A - Z)");
        Console.WriteLine("2. Precio (Menor a Mayor)");
        Console.WriteLine("3. Stock (Menor a Mayor)");
        Console.WriteLine("4. Orden original");
        Console.Write("Seleccione opción: ");

        int opc;
        while (!int.TryParse(Console.ReadLine(), out opc) || opc < 1 || opc > 4)
        {
            Console.Write("Opción no válida (1-4): ");
        }

        switch (opc)
        {
            case 1:
                productos.Sort((a, b) => string.Compare(a.Nombre, b.Nombre, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine("Productos ordenados de A a Z.");
                break;
            case 2:
                productos.Sort((a, b) => a.Precio.CompareTo(b.Precio));
                Console.WriteLine("Productos ordenados por precio (ascendente).");
                break;
            case 3:
                productos.Sort((a, b) => a.Stock.CompareTo(b.Stock));
                Console.WriteLine("Productos ordenados por stock (ascendente).");
                break;
            case 4:
                productos.Sort((a, b) => a.PosicionOriginal.CompareTo(b.PosicionOriginal));
                Console.WriteLine("Orden original restaurado.");
                break;
        }
        Console.WriteLine("Verifique los cambios ingresando a la opción 2 (Mostrar productos).");
    }

    // 10. VER GANANCIAS / VENTAS CON FECHA Y HORA
    static void MostrarGanancias()
    {
        Console.WriteLine("\n================ HISTORIAL DE VENTAS Y GANANCIAS ================");

        if (totalRegistrosVenta == 0)
        {
            Console.WriteLine("Aún no se han registrado ventas.");
            return;
        }

        string ventaActual = "";
        double totalGeneral = 0;
        double descuentosGenerales = 0;

        for (int i = 0; i < totalRegistrosVenta; i++)
        {
            if (codigosVenta[i] != ventaActual)
            {
                ventaActual = codigosVenta[i];

                Console.WriteLine($"\nCódigo: {codigosVenta[i]} | Fecha: {fechasVenta[i]:yyyy-MM-dd} | Hora: {fechasVenta[i]:HH:mm:ss}");
                Console.WriteLine(new string('-', 65));
            }

            double totalLinea = subtotalesVenta[i] - descuentosVenta[i];

            Console.WriteLine($"  - [{codigosProductoVenta[i]}] {nombresProductoVenta[i]} (x{cantidadesVenta[i]})");
            Console.WriteLine($"    Precio original: S/.{subtotalesVenta[i]:0.00}");
            Console.WriteLine($"    Descuento: -S/.{descuentosVenta[i]:0.00}");
            Console.WriteLine($"    Total: S/.{totalLinea:0.00}");

            descuentosGenerales += descuentosVenta[i];
            totalGeneral += totalLinea;
        }

        Console.WriteLine("\n================================================================");
        Console.WriteLine($"DESCUENTOS TOTALES: S/.{descuentosGenerales:0.00}");
        Console.WriteLine($"TOTAL ACUMULADO COBRADO: S/.{totalGeneral:0.00}");
        Console.WriteLine("================================================================");
    }

    // 11. HISTORIAL DE INGRESO DE STOCK CON FECHA Y HORA
    static void MostrarHistorialStock()
    {
        Console.WriteLine("\n================ HISTORIAL DE INGRESO DE STOCK ================");
        if (historialStock.Count == 0)
        {
            Console.WriteLine("No se ha registrado ningún reabastecimiento de stock aún.");
            return;
        }

        Console.WriteLine(string.Format("{0,-12} | {1,-8} | {2,-10} | {3,-38} | {4,-10}",
            "FECHA", "HORA", "CÓDIGO", "PRODUCTO", "CANTIDAD"));
        Console.WriteLine(new string('-', 88));

        foreach (var reg in historialStock)
        {
            Console.WriteLine(string.Format("{0,-12} | {1,-8} | {2,-10} | {3,-38} | +{4,-9}",
                reg.FechaHora.ToString("yyyy-MM-dd"),
                reg.FechaHora.ToString("HH:mm:ss"),
                reg.Codigo,
                reg.Nombre.Length > 38 ? reg.Nombre.Substring(0, 35) + "..." : reg.Nombre,
                reg.CantidadIngresada));
        }
    }

    // 12. REGISTRAR GASTO
    static void RegistrarGasto()
    {
        Console.WriteLine("\n--- REGISTRAR GASTO ---");

        string empleado;
        do
        {
            Console.Write("Empleado que realiza el gasto: ");
            empleado = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(empleado))
            {
                Console.WriteLine("Error: El empleado no puede quedar vacío.");
            }
        } while (string.IsNullOrWhiteSpace(empleado));

        double monto;
        Console.Write("Monto del gasto: S/ ");
        while (!double.TryParse(Console.ReadLine(), out monto) || monto <= 0)
        {
            Console.Write("Monto inválido. Ingrese un valor numérico mayor a 0: S/ ");
        }

        string descripcion;
        do
        {
            Console.Write("Descripción de la compra o pago: ");
            descripcion = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                Console.WriteLine("Error: La descripción no puede quedar vacía.");
            }
        } while (string.IsNullOrWhiteSpace(descripcion));

        string proposito;
        do
        {
            Console.Write("Propósito o motivo del gasto: ");
            proposito = Console.ReadLine().Trim();
            if (string.IsNullOrWhiteSpace(proposito))
            {
                Console.WriteLine("Error: El propósito no puede quedar vacío.");
            }
        } while (string.IsNullOrWhiteSpace(proposito));

        Gasto nuevoGasto = new Gasto
        {
            Empleado = empleado,
            Monto = monto,
            Descripcion = descripcion,
            Proposito = proposito,
            FechaHora = DateTime.Now
        };

        listaGastos.Add(nuevoGasto);
        GuardarGastos();

        Console.WriteLine("\n¡Gasto registrado exitosamente!");
        Console.WriteLine($"Fecha y hora de registro: {nuevoGasto.FechaHora:dd/MM/yyyy HH:mm}");
    }

    // 13. RESUMEN DEL DÍA (VENTAS, GASTOS Y RESULTADOS NETOS)
    static void ResumenDelDia()
    {
        DateTime fechaHoy = DateTime.Today;

        // Se agregó la fecha al encabezado de Gastos del Día
        Console.WriteLine($"\n========= GASTOS DEL DÍA: {fechaHoy:dd/MM/yyyy} =========");
        
        double totalGastosDia = 0;
        int contadorGasto = 1;
        bool huboGastos = false;

        foreach (var gasto in listaGastos)
        {
            if (gasto.FechaHora.Date == fechaHoy)
            {
                huboGastos = true;
                Console.WriteLine($"\n{contadorGasto}.");
                Console.WriteLine($"Empleado: {gasto.Empleado}");
                Console.WriteLine($"Monto: S/ {gasto.Monto:0.00}");
                Console.WriteLine($"Descripción: {gasto.Descripcion}");
                Console.WriteLine($"Propósito: {gasto.Proposito}");
                Console.WriteLine($"Fecha: {gasto.FechaHora:dd/MM/yyyy}");
                Console.WriteLine($"Hora: {gasto.FechaHora:HH:mm}");
                
                totalGastosDia += gasto.Monto;
                contadorGasto++;
            }
        }

        if (!huboGastos)
        {
            Console.WriteLine("\nNo se registraron gastos el día de hoy.");
        }

        Console.WriteLine($"\nTOTAL DE GASTOS DEL DÍA: S/ {totalGastosDia:0.00}\n");


        // Se agregó la fecha al encabezado de Resumen del Día
        Console.WriteLine($"========= RESUMEN DEL DÍA: {fechaHoy:dd/MM/yyyy} =========");

        double totalVentasDia = 0;
        double totalDescuentosDia = 0;

        for (int i = 0; i < totalRegistrosVenta; i++)
        {
            if (fechasVenta[i].Date == fechaHoy)
            {
                totalVentasDia += subtotalesVenta[i];
                totalDescuentosDia += descuentosVenta[i];
            }
        }

        double ingresosNetos = totalVentasDia - totalDescuentosDia;
        double resultadoNeto = ingresosNetos - totalGastosDia;

        Console.WriteLine($"Total de ventas: S/ {totalVentasDia:0.00}");
        Console.WriteLine($"Total de descuentos: S/ {totalDescuentosDia:0.00}");
        Console.WriteLine($"Ingresos netos: S/ {ingresosNetos:0.00}");
        Console.WriteLine($"Total de gastos: S/ {totalGastosDia:0.00}");
        Console.WriteLine($"Resultado neto: S/ {resultadoNeto:0.00}");
    }

    // GUARDADO Y CARGA DE DATOS
    static void GuardarProductos()
    {
        using (StreamWriter sw = new StreamWriter(archivoProductos, false))
        {
            foreach (Producto p in productos)
            {
                sw.WriteLine(string.Join("|",
                    p.Codigo,
                    p.Nombre.Replace("|", " "),
                    p.Marca.Replace("|", " "),
                    p.Precio.ToString(CultureInfo.InvariantCulture),
                    p.Stock,
                    p.Categoria.Replace("|", " "),
                    p.PosicionOriginal));
            }
        }
    }

    static void GuardarVentas()
    {
        using (StreamWriter sw = new StreamWriter(archivoVentas, false))
        {
            for (int i = 0; i < totalRegistrosVenta; i++)
            {
                sw.WriteLine(string.Join("|",
                    codigosVenta[i],
                    fechasVenta[i].ToString("O"),
                    codigosProductoVenta[i],
                    nombresProductoVenta[i].Replace("|", " "),
                    cantidadesVenta[i],
                    subtotalesVenta[i].ToString(CultureInfo.InvariantCulture),
                    descuentosVenta[i].ToString(CultureInfo.InvariantCulture)));
            }
        }
    }

    static void GuardarHistorialStock()
    {
        using (StreamWriter sw = new StreamWriter(archivoStock, false))
        {
            foreach (RegistroStock reg in historialStock)
            {
                sw.WriteLine(string.Join("|",
                    reg.FechaHora.ToString("O"),
                    reg.Codigo,
                    reg.Nombre.Replace("|", " "),
                    reg.CantidadIngresada));
            }
        }
    }

    static void GuardarGastos()
    {
        using (StreamWriter sw = new StreamWriter(archivoGastos, false))
        {
            foreach (Gasto g in listaGastos)
            {
                sw.WriteLine(string.Join("|",
                    g.Empleado.Replace("|", " "),
                    g.Monto.ToString(CultureInfo.InvariantCulture),
                    g.Descripcion.Replace("|", " "),
                    g.Proposito.Replace("|", " "),
                    g.FechaHora.ToString("O")));
            }
        }
    }

    static void CargarDatosGuardados()
    {
        if (File.Exists(archivoProductos))
        {
            List<Producto> productosGuardados = new List<Producto>();

            foreach (string linea in File.ReadAllLines(archivoProductos))
            {
                string[] datos = linea.Split('|');

                if (datos.Length == 7 &&
                    double.TryParse(datos[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double precio) &&
                    int.TryParse(datos[4], out int stock) &&
                    int.TryParse(datos[6], out int posicion))
                {
                    productosGuardados.Add(new Producto(
                        datos[0], datos[1], datos[2], precio, datos[5], posicion));

                    productosGuardados[productosGuardados.Count - 1].Stock = stock;
                }
            }

            if (productosGuardados.Count > 0)
            {
                productos = productosGuardados;
            }
        }

        if (File.Exists(archivoVentas))
        {
            foreach (string linea in File.ReadAllLines(archivoVentas))
            {
                string[] datos = linea.Split('|');

                if (datos.Length == 7 &&
                    DateTime.TryParse(datos[1], null, DateTimeStyles.RoundtripKind, out DateTime fecha) &&
                    int.TryParse(datos[4], out int cantidad) &&
                    double.TryParse(datos[5], NumberStyles.Any, CultureInfo.InvariantCulture, out double subtotal) &&
                    double.TryParse(datos[6], NumberStyles.Any, CultureInfo.InvariantCulture, out double descuento))
                {
                    if (totalRegistrosVenta < MAX_REGISTROS_VENTA)
                    {
                        codigosVenta[totalRegistrosVenta] = datos[0];
                        fechasVenta[totalRegistrosVenta] = fecha;
                        codigosProductoVenta[totalRegistrosVenta] = datos[2];
                        nombresProductoVenta[totalRegistrosVenta] = datos[3];
                        cantidadesVenta[totalRegistrosVenta] = cantidad;
                        subtotalesVenta[totalRegistrosVenta] = subtotal;
                        descuentosVenta[totalRegistrosVenta] = descuento;
                        totalRegistrosVenta++;
                    }

                    if (datos[0].StartsWith("VTA-") &&
                        int.TryParse(datos[0].Substring(4), out int numero) &&
                        numero > contadorVentas)
                    {
                        contadorVentas = numero;
                    }
                }
            }
        }

        if (File.Exists(archivoStock))
        {
            foreach (string linea in File.ReadAllLines(archivoStock))
            {
                string[] datos = linea.Split('|');

                if (datos.Length == 4 &&
                    DateTime.TryParse(datos[0], null, DateTimeStyles.RoundtripKind, out DateTime fecha) &&
                    int.TryParse(datos[3], out int cantidad))
                {
                    historialStock.Add(new RegistroStock
                    {
                        FechaHora = fecha,
                        Codigo = datos[1],
                        Nombre = datos[2],
                        CantidadIngresada = cantidad
                    });
                }
            }
        }
        
        if (File.Exists(archivoGastos))
        {
            foreach (string linea in File.ReadAllLines(archivoGastos))
            {
                string[] datos = linea.Split('|');

                if (datos.Length == 5 &&
                    double.TryParse(datos[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double monto) &&
                    DateTime.TryParse(datos[4], null, DateTimeStyles.RoundtripKind, out DateTime fecha))
                {
                    listaGastos.Add(new Gasto
                    {
                        Empleado = datos[0],
                        Monto = monto,
                        Descripcion = datos[2],
                        Proposito = datos[3],
                        FechaHora = fecha
                    });
                }
            }
        }
    }

    // CARGA DE LA BASE DE DATOS INICIAL
    static void CargarProductosIniciales()
    {
        int index = 0;
        void AddP(string c, string n, string m, double p, string cat)
        {
            productos.Add(new Producto(c, n, m, p, cat, index++));
        }

        // Guitarras (Nylon y Metal)
        AddP("GUI-001", "Guitarra para niño (Nylon)", "VOZZEX", 125, "Guitarras (Nylon y Metal)");
        AddP("GUI-002", "Guitarra para niño (Nylon)", "ALVERA", 200, "Guitarras (Nylon y Metal)");
        AddP("GUI-003", "Guitarra con corte y varilla (Nylon)", "VOZZEX", 150, "Guitarras (Nylon y Metal)");
        AddP("GUI-004", "Guitarra (Nylon)", "FEVER", 220, "Guitarras (Nylon y Metal)");
        AddP("GUI-005", "Guitarra (Nylon)", "ALVERA", 220, "Guitarras (Nylon y Metal)");
        AddP("GUI-006", "Guitarra (Nylon)", "MEMPHIS", 220, "Guitarras (Nylon y Metal)");
        AddP("GUI-007", "Guitarra EQ-4Bandas & Afinador (Nylon)", "FEVER", 330, "Guitarras (Nylon y Metal)");
        AddP("GUI-008", "Guitarra de Caoba y Pino (Nylon)", "SJ", 420, "Guitarras (Nylon y Metal)");
        AddP("GUI-009", "Guitarra delgada EQ (Nylon)", "PALMER", 450, "Guitarras (Nylon y Metal)");
        AddP("GUI-010", "Guitarra modelo Godin EQ (Nylon)", "MCALLISTER", 550, "Guitarras (Nylon y Metal)");
        AddP("GUI-011", "Guitarra clásica sin corte y varilla (Nylon)", "CLEVAN", 400, "Guitarras (Nylon y Metal)");
        AddP("GUI-012", "Guitarra clásica con corte y varilla (Nylon)", "CLEVAN", 450, "Guitarras (Nylon y Metal)");
        AddP("GUI-013", "Guitarra clásica sin corte (Nylon)", "YAMAHA", 600, "Guitarras (Nylon y Metal)");
        AddP("GUI-014", "Guitarra (Nylon)", "ORTEGA", 550, "Guitarras (Nylon y Metal)");
        AddP("GUI-015", "Pack guitarra clásica (Nylon)", "ORTEGA", 600, "Guitarras (Nylon y Metal)");
        AddP("GUI-016", "Guitarra clásica sin corte blanco (Nylon)", "ORTEGA", 800, "Guitarras (Nylon y Metal)");
        AddP("GUI-017", "Guitarra clásica cedro sin corte marrón (Nylon)", "ORTEGA", 800, "Guitarras (Nylon y Metal)");
        AddP("GUI-018", "Guitarra EQ con corte (Nylon)", "ORTEGA", 980, "Guitarras (Nylon y Metal)");
        AddP("GUI-019", "Guitarra EQ (Nylon)", "TAKAMINE", 1250, "Guitarras (Nylon y Metal)");
        AddP("GUI-020", "Guitarra EQ esqueleto (Nylon)", "DONNER", 1100, "Guitarras (Nylon y Metal)");
        AddP("GUI-021", "Guitarra sólida EQ (Electroacústica)", "TAGIMA", 1650, "Guitarras (Nylon y Metal)");
        AddP("GUI-022", "Guitarra acústica 39\" (Metal)", "FEVER", 250, "Guitarras (Nylon y Metal)");
        AddP("GUI-023", "Guitarra acústica 39\" (Metal)", "MEMPHIS", 290, "Guitarras (Nylon y Metal)");
        AddP("GUI-024", "Guitarra acústica Jumbo EQ (Metal)", "VOZZEX", 400, "Guitarras (Nylon y Metal)");
        AddP("GUI-025", "Guitarra acústica Jumbo EQ (Metal)", "WASHBURN", 480, "Guitarras (Nylon y Metal)");
        AddP("GUI-026", "Guitarra acústica EQ metal (Negro)", "FREEMAN", 430, "Guitarras (Nylon y Metal)");
        AddP("GUI-027", "Guitarra acústica EQ metal (Natural)", "FREEMAN", 430, "Guitarras (Nylon y Metal)");
        AddP("GUI-028", "Guitarra acústica Semijumbo (Metal)", "ASTURIAS", 450, "Guitarras (Nylon y Metal)");
        AddP("GUI-029", "Guitarra acústica Semijumbo (Metal)", "ASTURIAS", 600, "Guitarras (Nylon y Metal)");

        // Teclados
        AddP("TEC-001", "Teclado 61K", "KEYBOARD", 385, "Teclados");
        AddP("TEC-002", "Teclado 61K", "KEYBOARD", 420, "Teclados");
        AddP("TEC-003", "Teclado 61K con sensibilidad", "KEYBOARD", 550, "Teclados");
        AddP("TEC-004", "Teclado 61K con sensibilidad + funda", "VOZZEX", 500, "Teclados");
        AddP("TEC-005", "Teclado 61K con sensibilidad", "DONNER", 650, "Teclados");
        AddP("TEC-006", "Teclado 61K con sensibilidad", "CASIO", 700, "Teclados");
        AddP("TEC-007", "Teclado 61K con sensibilidad", "CASIO", 1000, "Teclados");
        AddP("TEC-008", "Teclado 61K con sensibilidad (Workstation)", "CASIO", 1350, "Teclados");
        AddP("TEC-009", "Teclado 61K", "YAMAHA", 680, "Teclados");
        AddP("TEC-010", "Teclado 61K con sensibilidad", "YAMAHA", 930, "Teclados");
        AddP("TEC-011", "Teclado 61K con sensibilidad + MIDI-USB", "YAMAHA", 1600, "Teclados");
        AddP("TEC-012", "Teclado 61K (Workstation)", "YAMAHA", 3500, "Teclados");
        AddP("TEC-013", "Teclado 61K (Workstation)", "YAMAHA", 5600, "Teclados");

        // Flautas, Melódicas y Armónicas
        AddP("VIE-001", "Flauta dulce econ (Color hueso)", "LK", 12, "Flautas, Melódicas y Armónicas");
        AddP("VIE-002", "Flauta dulce Shepher", "BALDESSARE", 18, "Flautas, Melódicas y Armónicas");
        AddP("VIE-003", "Flauta dulce Shepher (Color hueso)", "SHEPHER", 20, "Flautas, Melódicas y Armónicas");
        AddP("VIE-004", "Flauta dulce Yamaha", "YAMAHA", 28, "Flautas, Melódicas y Armónicas");
        AddP("VIE-005", "Flauta dulce Hohner (Color hueso)", "HOHNER", 38, "Flautas, Melódicas y Armónicas");
        AddP("VIE-006", "Melódica flauta 13 teclas", "LK", 20, "Flautas, Melódicas y Armónicas");
        AddP("VIE-007", "Melódica 32K (Tela)", "LK", 55, "Flautas, Melódicas y Armónicas");
        AddP("VIE-008", "Melódica 37K (Tela)", "LK", 60, "Flautas, Melódicas y Armónicas");
        AddP("VIE-009", "Melódica 37K (Lona)", "MELODY", 70, "Flautas, Melódicas y Armónicas");
        AddP("VIE-010", "Melódica 37K (Lona)", "FEVER", 75, "Flautas, Melódicas y Armónicas");
        AddP("VIE-011", "Melódica 37K (Lona)", "CALIFORNIA", 80, "Flautas, Melódicas y Armónicas");
        AddP("VIE-012", "Melódica 37K (Cuero)", "SUZUKI", 120, "Flautas, Melódicas y Armónicas");
        AddP("VIE-013", "Melódica 37K (Lona)", "HOHNER", 320, "Flautas, Melódicas y Armónicas");
        AddP("VIE-014", "Armónica 10H ABS (Colores)", "VILLEGAS", 12, "Flautas, Melódicas y Armónicas");
        AddP("VIE-015", "Armónica 24H ABS", "LK", 20, "Flautas, Melódicas y Armónicas");
        AddP("VIE-016", "Armónica 10H", "BEE", 15, "Flautas, Melódicas y Armónicas");
        AddP("VIE-017", "Armónica 24H", "BEE", 20, "Flautas, Melódicas y Armónicas");
        AddP("VIE-018", "Armónica 16H", "BEE", 35, "Flautas, Melódicas y Armónicas");
        AddP("VIE-019", "Armónica 10H metal", "SWAN", 45, "Flautas, Melódicas y Armónicas");
        AddP("VIE-020", "Armónica 24H metal", "EASTTOP", 65, "Flautas, Melódicas y Armónicas");
        AddP("VIE-021", "Armónica doble metal", "EASTTOP", 85, "Flautas, Melódicas y Armónicas");
        AddP("VIE-022", "Armónica cromática metal", "TOWER", 65, "Flautas, Melódicas y Armónicas");
        AddP("VIE-023", "Armónica 10H metal", "HOHNER", 60, "Flautas, Melódicas y Armónicas");

        // Vientos Metal (Trompetas, Cornetas y Trombones)
        AddP("MET-001", "Trompeta dorada 2 puentes", "CALIFORNIA", 480, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-002", "Trompeta tricolor 2 puentes", "CALIFORNIA", 520, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-003", "Trompeta dorada 2 puentes", "FEVER", 500, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-004", "Trompeta dorada 1 puente", "JINBAO", 600, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-005", "Trompeta dorada 2 puentes", "JINBAO", 800, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-006", "Trompeta pocket dorada", "FEVER", 600, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-007", "Trompeta niquelada 2 puentes", "CALIFORNIA", 550, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-008", "Trompeta tricolor 2 puentes", "JINBAO", 950, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-009", "Trompeta plateada 2 puentes", "FEVER", 700, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-010", "Trompeta plateada 1 puente", "JINBAO", 900, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-011", "Trompeta plateada 2 puentes", "JINBAO", 1050, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-012", "Corneta nacional + boquilla", "NACIONAL", 135, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-013", "Corneta importada + boquilla + estuche", "GRANDPRIX", 160, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-014", "Corneta importada + boquilla (Bb)", "HOHFFER", 180, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-015", "Trombón", "CALIFORNIA", 580, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-016", "Trombón (Económico / Calidad)", "FEVER", 600, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-017", "Trombón lineal (Tubo grueso)", "PHILADELPHI", 600, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-018", "Trombón lineal (Tubo delgado)", "JINBAO", 780, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-019", "Trombón lineal (Tubo grueso)", "JINBAO", 850, "Vientos Metal (Trompetas, Cornetas y Trombones)");
        AddP("MET-020", "Trombón transpositor (Tubo grueso)", "JINBAO", 1500, "Vientos Metal (Trompetas, Cornetas y Trombones)");
    }
}
