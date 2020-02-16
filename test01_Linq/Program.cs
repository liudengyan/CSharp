using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test01_Linq
{
    public class MartialArtsMaster
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Menpai { get; set; }

        public string Kongfu { get; set; }

        public int Level { get; set; }
    }

    class Kongfu
    {
        public int KongfuId { get; set; }

        public string KongfuName { get; set; }

        public int Lethality { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {
            //初始化武林高手。
            var master = new List<MartialArtsMaster>()
            {
                new MartialArtsMaster(){Id = 1,Name = "黄蓉",Age = 8,Menpai = "丐帮",Kongfu = "打狗棒法",Level = 9 },
                new MartialArtsMaster(){Id = 2,Name = "洪七公",Age = 70,Menpai = "丐帮",Kongfu = "打狗棒法",Level = 10 },
                new MartialArtsMaster(){Id = 3,Name = "郭靖",Age = 22,Menpai = "丐帮",Kongfu = "降龙十八掌",Level = 10 },
                new MartialArtsMaster(){Id = 4,Name = "任我行",Age = 50,Menpai = "明教",Kongfu = "葵花宝典",Level = 1 },
                new MartialArtsMaster(){Id = 5,Name = "东方不败",Age = 35,Menpai = "明教",Kongfu = "葵花宝典",Level = 10 },
                new MartialArtsMaster(){Id = 6,Name = "林平之",Age = 23,Menpai = "华山",Kongfu = "葵花宝典",Level = 7 },
                new MartialArtsMaster(){Id = 7,Name = "岳不群",Age = 50,Menpai = "华山",Kongfu = "葵花宝典",Level = 9 }
            };

            //初始化武学
            var Kongfu = new List<Kongfu>()
            {
                new Kongfu(){KongfuId = 1, KongfuName = "打狗棒法" , Lethality = 90},
                new Kongfu(){KongfuId = 2, KongfuName = "降龙十八掌" , Lethality = 95},
                new Kongfu(){KongfuId = 3, KongfuName = "葵花宝典" , Lethality = 100}
            };


            //在武林大侠中选出Level大于8级且门派为丐帮的大侠
            var sequence = from a in master
                           where a.Level > 8 && a.Menpai == "丐帮"
                           select a;
            var sequenceMethod = master.Where(x => x.Level > 8 && x.Menpai == "丐帮");
            //过滤所学武功杀伤力大于90的大侠，且按等级进行升序排序。
            var sequence1 = (from a in master
                            from b in Kongfu
                            where b.Lethality > 90 && a.Kongfu == b.KongfuName
                            orderby a.Level
                            select new
                            {
                                a.Id,
                                a.Name,
                                a.Kongfu,
                                a.Level,
                                a.Menpai,
                            });
            var sequence1Method = master.SelectMany(k => Kongfu, (m, k) => new { mt = m, kf = k })
                .Where(x => x.kf.Lethality > 90 && x.kf.KongfuName == x.mt.Kongfu)
                .OrderBy(m => m.mt.Level)
                .Select(m => new
                {
                    m.mt.Id,
                    m.mt.Name,
                    m.mt.Kongfu,
                    m.mt.Level,
                    m.mt.Menpai,
                });

            //武林排行榜
            int i = 0;
            var sequence2 = from m in master
                            join k in Kongfu on m.Kongfu equals k.KongfuName
                            //from k in Kongfu
                            //where m.Kongfu == k.KongfuName
                            orderby m.Level * k.Lethality descending, m.Age descending, m.Name
                            select new
                            {
                                m.Name,
                                m.Id,
                                m.Kongfu,
                                weili = m.Level * k.Lethality,
                                paiming = ++i
                            };
            var sequence2Method = master.SelectMany(m => Kongfu, (m, k) => new { mt = m, kf = k }).Where(m => m.mt.Kongfu == m.kf.KongfuName).OrderByDescending(x => x.kf.Lethality * x.mt.Level).ThenByDescending(x => x.mt.Age).Select(
                x => new { 
                    x.mt.Id,
                    x.mt.Name,
                    V = x.mt.Level *x.kf.Lethality,
                    x.mt.Kongfu,
                    paimign = ++i
                });

            var sequenceM = from m in master
                            where m.Level > 8
                            orderby m.Level descending
                            select m;
            var sequenceK = from k in Kongfu
                            where k.Lethality > 90
                            orderby k.Lethality descending
                            select k;

            var sequence3 = from m in sequenceM
                            from k in sequenceK
                            where m.Kongfu == k.KongfuName
                            //join k in Kongfu on m.Kongfu equals k.KongfuName
                            orderby m.Level * k.Lethality descending
                            select new
                            {
                                paiming = ++i,
                                m.Id,
                                m.Name,
                                m.Age,
                                V = m.Level * k.Lethality
                            };

            var sequence4 = from k in Kongfu
                            join m in master on k.KongfuName equals m.Kongfu 
                            into groups
                            orderby groups.Count() descending
                            select new
                            {
                                V = ++i,
                                k.KongfuId,
                                k.KongfuName,
                                V1 = groups.Count()
                            };
            //Intersect:相等比较器，生成两个序列的交集
            var sequence5 = (from m in master
                             where m.Menpai == "明教" || m.Menpai == "华山"
                             select m).Intersect(from m in master where m.Kongfu == "葵花宝典" select m);
            foreach (var item in sequence4)
            {
                Console.WriteLine(item);
            }
            return;
            string[] languane = { "java", "C#", "C++", "python", "golang" };
            var query = from a in languane
                        group a by a.Length into g
                        orderby g.Key
                        select g;
            foreach (var items in query)
            {
                Console.WriteLine(items.Key);
                foreach (var item in items)
                {
                    Console.WriteLine(item);
                }
            }
        }
    }
}
