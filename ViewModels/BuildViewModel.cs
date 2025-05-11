using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.ViewModels
{
    public class SelectItemWithImage
    {
        public string Id       { get; set; } = null!;
        public string Name     { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
    }

    public class BuildViewModel
    {
        // user picks
        public int? SelectedClassId    { get; set; }
        public int? SelectedSubclassId { get; set; }
        public int? SelectedArmorId    { get; set; }
        public int? SelectedWeaponId   { get; set; }
        public int? PrimaryStatId      { get; set; }
        public int? SecondaryStatId    { get; set; }
        public int? SelectedFocusId    { get; set; }

        // image‐based pickers (only these two)
        public IEnumerable<SelectItemWithImage> Classes    { get; set; } 
            = new List<SelectItemWithImage>();
        public IEnumerable<SelectItemWithImage> Subclasses { get; set; } 
            = new List<SelectItemWithImage>();

        // standard dropdowns
        public IEnumerable<SelectListItem> Armors   { get; set; } 
            = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Weapons  { get; set; } 
            = new List<SelectListItem>();

        // still plain lists for stats & focus
        public IEnumerable<SelectListItem> Stats        { get; set; } 
            = new List<SelectListItem>();
        public IEnumerable<SelectListItem> FocusOptions { get; set; } 
            = new List<SelectListItem>();

        // final recommendation
        public BuildMapping? Recommendation { get; set; }
    }
}
