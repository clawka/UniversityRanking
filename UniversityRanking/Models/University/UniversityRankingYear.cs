using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniversityRanking.Models.University;

public partial class UniversityRankingYear
{
    public int? UniversityId { get; set; }

    [Required]
    public int? RankingCriteriaId { get; set; }

    [Required(ErrorMessage = "Year is required.")]
    [Range(2017, int.MaxValue, ErrorMessage = "Year must be above 2016.")]
    public int? Year { get; set; }

    [Required]
    public int? Score { get; set; }

    public virtual RankingCriterion? RankingCriteria { get; set; }

    public virtual University? University { get; set; }
}
