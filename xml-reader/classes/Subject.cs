using System;
using System.Collections.Generic;
using System.Text;

namespace xml_reader.classes
{
    public record Object(
    string IDSubg,
    string Disc,
    string? Type,
    string Chair,
    string IdPrep,
    string Prep,
    string[] IdGroups,
    string[] Groups,
    string? Week,
    string Day,
    string Less, 
    string Buildings,
    string[] Rooms
);
}
