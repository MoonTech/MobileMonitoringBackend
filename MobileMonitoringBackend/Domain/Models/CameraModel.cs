﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MobileMonitoringBackend.Domain.Models;

[Table("Cameras")]
[Index(nameof(RoomName),nameof(CameraName), IsUnique = true)]
public class CameraModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public Guid Id { get; set; }
    
    public string CameraName { get; set; }
    public string CameraToken { get; set; }
    public bool AcceptationState { get; set; }

    [ForeignKey("RoomName")]
    public RoomModel? Room { get; set; }
    public string RoomName { get; set; }
}