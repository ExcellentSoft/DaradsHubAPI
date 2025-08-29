using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaradsHubAPI.Domain.Entities;
#nullable disable
public partial class Logapp_error
{
    public int id { get; set; }

    [Column(TypeName = "text")]
    public string error_message { get; set; }

    [StringLength(50)]
    public string error_pagename { get; set; }

    [StringLength(50)]
    public string error_methodname { get; set; }

    public string app_userID { get; set; }

    [StringLength(20)]
    public string app_Action { get; set; }

    public int? error_prioritycode { get; set; }

    [Column(TypeName = "date")]
    public DateTime? error_date { get; set; }

    public TimeSpan? error_time { get; set; }
}
