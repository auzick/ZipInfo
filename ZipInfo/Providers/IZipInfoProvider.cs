﻿using System.Collections.Generic;

namespace ZipInfo.Providers
{
    public interface IZipInfoProvider
    {
        IEnumerable<IZipCode> GetAll();

        IZipCode Get(int zipCode);

        bool Set(IZipCode zipCode);

        string Reload(bool force);



    }
}