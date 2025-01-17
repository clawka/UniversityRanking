﻿using System;
using System.Collections.Generic;

namespace UniversityRanking.Models.University;

public partial class RankingSystem
{
    public int Id { get; set; }

    public string? SystemName { get; set; }

    public virtual ICollection<RankingCriterion> RankingCriteria { get; set; } = new List<RankingCriterion>();
}
