using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace BP.UnitTesting
{
    /// <summary>
    /// 全局
    /// </summary>
    public class Glo
    {
        /// <summary>
        /// 根据enName获取单元测试实例.
        /// </summary>
        /// <param name="enName"></param>
        /// <returns></returns>
        public static BP.UnitTesting.TestBase GetTestEntity(string enName)
        {
            ArrayList al = null;
            al = BP.En.ClassFactory.GetObjects("BP.UnitTesting.TestBase");
            foreach (Object obj in al)
            {
                BP.UnitTesting.TestBase en = null;
                try
                {
                    en = obj as BP.UnitTesting.TestBase;
                    if (en == null)
                        continue;
                    string s = en.Title;
                    if (en == null)
                        continue;
                }
                catch
                {
                    continue;
                }

                if (en.ToString() == enName)
                    return en;
            }

            throw new Exception("err@单元测试名称拼写错误或者不存在["+enName+"]");

        }
    }
}
