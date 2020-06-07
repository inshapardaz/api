﻿using Inshapardaz.Domain.Models;
using Inshapardaz.Functions.Views;
using System;

namespace Inshapardaz.Functions.Converters
{
    public class PageRendererArgs<T>
    {
        public Page<T> Page { get; set; }

        public PagedRouteArgs RouteArguments { get; set; }

        // TODO :  Depricate
        public Func<int, int, string, LinkView> LinkFunc { get; set; }

        public Func<int, int, int, string, string, LinkView> LinkFuncWithParameter { get; set; }

        public Func<int, int, int, int, string, string, LinkView> LinkFuncWithParameterEx { get; set; }
    }

    public class PagedRouteArgs
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public string Query { get; set; }
    }
}
