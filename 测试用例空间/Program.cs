using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            foreach (var item in source)
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
        /// <summary>
        /// 实现了Any的重载，确定一个序列是否包含一个值,返回bool类型。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            //调用where的方法判断序列是否有值并返回此值，这样调用他的子类就可以确定次序列是否有此项。
            return source.Where(predicate).GetEnumerator().MoveNext();
        }
        /// <summary>
        /// 实现count的核心
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int Count<T>(this IEnumerable<T> source)
        {
            var count = 0;
            while (source.GetEnumerator().MoveNext()!=false)
            {
                count++;
            }

            return count;
        }
        /// <summary>
        /// 实现count的函数重载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int Count<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).Count();
        }
        /// <summary>
        /// 简单版本的实现求和
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int Sum(this IEnumerable<int> source)
        {
            int sum = 0;
            foreach (var item in source)
            {
                sum = sum + item;
            }

            return sum;
        }
        /// <summary>
        /// 种子求和，可以实现一个固定的源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int Sum(this IEnumerable<int> source,int seed)
        {
            int sum = seed;
            foreach (var item in source)
            {
                sum = sum + item;
            }
            return sum;
        }
        /// <summary>
        /// 实现一个累加器的方法，这种方式更加的通用，对于任意类型都可以实现特的函数方式的累加。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">方法调用者</param>
        /// <param name="func">一个方法函数</param>
        /// <returns></returns>
        public static T Aggregate<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();
            var sum = enumerator.Current;
            while (enumerator.MoveNext())
            {
                sum = func(sum, enumerator.Current);
            }
            return sum;
        }


    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(SequenceFormConsole().Aggregate((x, y) => x + "," + y));
            return;
            /*
            //Linq查询集语法
            int[] a = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int[] b = { 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            var sum = from x in a
                      from y in b
                      select new { x, y, sum = x + y };
            foreach (var ss in sum)
            {
                Console.WriteLine(ss);
            }
            
            return;
            //OrderBy(排序)方法:这里的orderby与thenby方法并不会具体实现，这种排序方法实际上非常复杂，你可以尝试去阅读一些源码，这里索
            //要知道的只有 1、排序方法非常消耗计算机资源，不只是C#，在java，object-c等等语言上都是这样，但是这种消耗的资源我们会尽量的
            //去少消耗 2、了解一些排序的原理，这里的排序不会去根据不同的lambda函数一次一次的遍历序列，直至返回排序的序列，而是等待着，根
            //根据所有的lambda函数，去中和出一个满足所有lambda函数的排序规则，一次性的遍历出满足所有的lamabd函数的序列并返回，这种方式
            //无疑是效率很高的一种方法，不必去多次排序序列。3、合理使用OrderBy与ThenBy。
            var sequences = SequenceFormConsole().OrderBy(x => x.Length).ThenBy(x => x);
            foreach (var item in sequences)
            {
                Console.WriteLine(item);
            }
            return;
            
            //count方法
            Console.WriteLine(SequenceFormConsole().Count(x=>x.Contains("hello")));
            return;
            //any方法
            Console.WriteLine(SequenceFormConsole().Any(x=>x=="hello"));

            return;
            var input = SequenceFormConsole().Select(x => int.Parse(x));
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
            */
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
            string text = Console.ReadLine();
            while (text != "done")
            {
                yield return text;
                text = Console.ReadLine();
            }
        }
    }
}
