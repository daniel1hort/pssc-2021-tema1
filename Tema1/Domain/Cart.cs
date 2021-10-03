using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tema1.Fake;

namespace Tema1.Domain
{
    public record Cart(User User, IEnumerable<(Product Product, int Quantity)> Items)
    {
        public Cart(User user) : this(user, Enumerable.Empty<(Product, int)>()) { }
    }
}
