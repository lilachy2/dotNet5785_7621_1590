﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class VolunteerFilterCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolInList> s_enums =
            Enum.GetValues(typeof(BO.VolInList)).Cast<BO.VolInList>();

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class DistanceTypes : IEnumerable
    {
        static readonly IEnumerable<BO.DistanceType> distanceTypes =
            Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        public IEnumerator GetEnumerator() => distanceTypes.GetEnumerator();
    }

    internal class Roles : IEnumerable
    {
        static readonly IEnumerable<BO.Role> roles =
            Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();

        public IEnumerator GetEnumerator() => roles.GetEnumerator();
    }

    internal class Calltype : IEnumerable
    {
        static readonly IEnumerable<BO.Calltype> callTypes =
            Enum.GetValues(typeof(BO.Calltype)).Cast<BO.Calltype>();

        public IEnumerator GetEnumerator() => callTypes.GetEnumerator();
    }
    internal class ClosedCallInListEnum : IEnumerable
    {
        static readonly IEnumerable<BO.ClosedCallInListEnum> ClosedCallInListEnums =
            Enum.GetValues(typeof(BO.ClosedCallInListEnum)).Cast<BO.ClosedCallInListEnum>();

        public IEnumerator GetEnumerator() => ClosedCallInListEnums.GetEnumerator();
    }
    internal class CallInListFieldEnum : IEnumerable
    {
        static readonly IEnumerable<BO.CallInListField> ClosedCallInListEnums =
            Enum.GetValues(typeof(BO.CallInListField)).Cast<BO.CallInListField>();

        public IEnumerator GetEnumerator() => ClosedCallInListEnums.GetEnumerator();
    }
}
