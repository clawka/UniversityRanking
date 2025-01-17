﻿using System;
using System.Collections.Generic;

namespace UniversityRanking.Models.University;

public partial class RankingCriterion
{
    public int Id { get; set; }

    public int? RankingSystemId { get; set; }

    public string? CriteriaName { get; set; }

    public virtual RankingSystem? RankingSystem { get; set; }
}
