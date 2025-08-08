namespace DataStructuresAlgorithms
{
    public class SeasonTwoOfBook
    {
        public static void Run()
        {
            Console.WriteLine("Hello Algorithem");

            int[] a = new int[5];//0,0,0,0,0
            string[] names = new string[5];//null,null,null,null,null

            int[] b = new int[] { 1, 2, 3, 4, 5 };
            string[] c = new string[] { "A", "B", "C" };


            var arr = new[] { 10, 20, 30, 40 };
            Console.WriteLine(arr[0]);

            Console.WriteLine(arr.Length);
            Console.WriteLine(arr.Rank);

            Type t = arr.GetType();
            Console.WriteLine(t.IsArray);
            Console.WriteLine(t);
        }
    }
}