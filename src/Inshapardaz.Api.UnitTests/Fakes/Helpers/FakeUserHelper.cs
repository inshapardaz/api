using System;
using Inshapardaz.Domain.Helpers;

namespace Inshapardaz.Api.UnitTests.Fakes.Helpers
{
    public class FakeUserHelper : IUserHelper
    {
        private Guid _userId = Guid.Empty;
        private bool _authenticated;
        private bool _admin;
        private bool _contributor;
        private bool _reader;

        public FakeUserHelper WithUserId(Guid userId)
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

        public bool IsLoggedIn
        {
            get { return _reader || _contributor || _authenticated || _admin; }
        }

        public Guid GetUserId()
        {
            return _userId;
        }
    }
}
