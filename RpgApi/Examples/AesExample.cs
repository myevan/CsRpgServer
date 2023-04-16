using Rpg.Helpers;

namespace Rpg.Examples
{
    public class AesExample
    {
        public static void Run()
        {
            var key = AesHelper.GenerateKeyString();
            var iv = AesHelper.GenerateIvString(key);
            Console.WriteLine("# AES");
            Console.WriteLine(key);
            Console.WriteLine(iv);
            Console.WriteLine("--");
        }
    }
}
