using System;
using System.Collections.Generic;
using System.Text;

namespace xml_reader.classes
{
    public record ScheduleEntry(
    string SubjectId,       // IDSubg
    string Discipline,      // disc
    string? Type,           // type 
    string TeacherId,       // id_prep
    string GroupId,         // id_group 
    string? Week,           // week
    string Day,             // day 
    string LessonNumber,    // less
    string Building,        // buildings
    string Room             // rooms 
    );
}
