﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace PL;
//internal/*public*/ class Calltype : IEnumerable
//{
//    static readonly IEnumerable<BO.Calltype> s_enums =
//(Enum.GetValues(typeof(BO.Calltype)) as IEnumerable<BO.Calltype>)!;
//    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
//}
namespace PL
{
    internal class VolunteerFilterCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolInList> s_enums =
            (Enum.GetValues(typeof(BO.VolInList)) as IEnumerable<BO.VolInList>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}







