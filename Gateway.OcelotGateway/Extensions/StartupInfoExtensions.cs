namespace Gateway.OcelotGateway.Extensions
{
    public static class StartupInfoExtensions
    {
        private static readonly Dictionary<string, ConsoleColor> ServiceColors = new()
        {
            ["Gateway.OcelotGateway"] = ConsoleColor.Blue,
            ["AuthService"] = ConsoleColor.Cyan,
            ["InventarioService"] = ConsoleColor.Yellow,
            ["Inventory.Api"] = ConsoleColor.Green,
            ["ProductoService"] = ConsoleColor.Magenta,
            ["TransaccionService"] = ConsoleColor.Red
        };

        public static void PrintStartupInfo(this WebApplicationBuilder builder)
        {
            var appName = builder.Environment.ApplicationName;
            var env = builder.Environment.EnvironmentName;

            var color = ServiceColors.GetValueOrDefault(
                appName,
                ConsoleColor.White);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");

            Console.ForegroundColor = color;
            Console.Write(appName);

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("] ");

            Console.ForegroundColor = env switch
            {
                "Development" => ConsoleColor.Green,
                "Staging" => ConsoleColor.Yellow,
                "Production" => ConsoleColor.Red,
                _ => ConsoleColor.White
            };

            Console.WriteLine(env);

            Console.ResetColor();
        }
    }
}
