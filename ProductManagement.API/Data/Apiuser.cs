using System;
using System.Collections.Generic;

namespace ProductManagement.API.Data;

public partial class Apiuser
{
    public int ClientId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
