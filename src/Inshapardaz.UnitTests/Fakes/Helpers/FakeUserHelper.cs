using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Inshapardaz.Helpers;

namespace Inshapardaz.UnitTests.Fakes.Helpers
{
    public class FakeUserHelper : IUserHelper
    {
        private string _userId = null;
        private bool _authenticated;
        private bool _admin;
        private bool _contributor;
        private bool _reader;

        public FakeUserHelper WithUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public FakeUserHelper Authenticated()
        {
            _authenticated = true;
            return this;
        }

        public FakeUserHelper AsAdmin()
        {
            _admin = true;

            return this;
        }

        public FakeUserHelper AsContributor()
        {
            _contributor = true;

            return this;
        }

        public FakeUserHelper AsReader()
        {
            _reader= true;

            return this;
        }


        public bool IsAdmin
        {
            get
            {
                return _admin;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _authenticated;
            }
        }

        public bool IsContributor
        {
            get
            {
                return _contributor;
            }
        }

        public bool IsReader
        {
            get
            {
                return _reader;
            }
        }

        public string GetUserId()
        {
            return _userId;
        }
    }
}
