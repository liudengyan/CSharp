using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace 测试用例空间
{
    /// <summary>
    /// Linq的核心原理探究
    /// </summary>
    public static class MyLinqImplementation
    {
        //where的仿照实现

        /// <summary>
        /// 这是一种无参数的序列判断，当然这正方式并不好，我们要自己的定义判断函数，这就需要一种方法，也就是委托的实现
        /// 这种方式能极大的满足lambda函数的可重用性质
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<string> Where(this IEnumerable<string> source)
        {
            foreach (string item in source)
            {
                if (item.Length<2)
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 这是一个Where的实现，也是一个Where的重载方法，是上面的重用扩展，这种方法的扩展性会更好，但还不是最好
        /// 由这个方法可知，这种只能传递一个String的参数，所以扩展性不能说最好，为了满足可重用性的原则，我们需要
        /// 将这个方法的泛型改为通用泛型T
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<string> Where(this IEnumerable<string> source,Func<string,bool> predicate)
        {
            foreach (string item in source)
            {
                //这里实现了委托的传递参数，函数为：x=>x.Length<2
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 这种方式第三种形式，泛型的重用,实现了自定义泛型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (T item in source)
            {
                //这里实现了委托的传递参数，函数为：x=>x.Length<2
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// 这是一个select的核心实现,当然，这种实现是无扩展的，为了重用性，这里应该改为自定义泛型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<string> Select(this IEnumerable<int> source,Func<int,string> selector)
        {
            foreach (int item in source)
            {
                yield return selector(item);
            }
        }
        /// <summary>
        /// 自定义泛型实现select
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> Select<TSource,TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            foreach (TSource item in source)
            {
                yield return selector(item);
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var input = SequenceFormConsole();
            foreach (var item in input)
            {
                Console.WriteLine($"\t{item}");
            }
            return;
            var sequence = GenerateSequence2().Where(x => x%5 == 0).Select((x,index)=> 
                new 
                {
                    index,
                    formatted = x.ToString()
                });
            foreach (var item in sequence)
            {
                Console.WriteLine(item);
            }

            return;
            //这是一种仿照foreach的遍历集合的方法
            List<int> s = new List<int>{1,5,3,7,23};
            var enumerator = s.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current);
            }
        }

        public static IEnumerable<string> GenerateSequence()
        {
            var i = 0;
            while (i++<100)
            {
                yield return i.ToString();
            }
        }

        public static IEnumerable<int> GenerateSequence2()
        {
            var i = 0;
            while (i++ < 100)
            {
                yield return i;
            }
        }

        public static IEnumerable<string> SequenceFormConsole()
        {
            string text = default(string);
            while (text != "done")
            {
                yield return text;
                text = Console.ReadLine();
            }
        }
    }
}
