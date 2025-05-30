// ViewModels/BuildViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.ViewModels
{
    public class SelectItemWithImage
    {
        public string Id       { get; set; } = string.Empty;
        public string Name     { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class BuildViewModel
    {
        // Pick-lists for the form
        public IEnumerable<SelectItemWithImage> Classes      { get; set; } = new List<SelectItemWithImage>();
        public IEnumerable<SelectItemWithImage> Subclasses   { get; set; } = new List<SelectItemWithImage>();
        public IEnumerable<SelectListItem>       Armors       { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem>       Weapons      { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem>       FocusOptions { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem>       ChargeTypes  { get; set; } = new List<SelectListItem>();

        // User’s selections
        public int? SelectedClassId       { get; set; }
        public int? SelectedSubclassId    { get; set; }
        public int? SelectedArmorId       { get; set; }
        public int? SelectedWeaponHash    { get; set; }
        public int? SelectedFocusId       { get; set; }
        public int? SelectedChargeTypeId  { get; set; }

        // Generated loadouts
        public Dictionary<string, List<ModDef>>     ModLoadout       { get; set; } = new Dictionary<string, List<ModDef>>();
        public Dictionary<string, List<Aspect>>     AspectLoadout    { get; set; } = new Dictionary<string, List<Aspect>>();
        public Dictionary<string, List<Fragment>>   FragmentLoadout  { get; set; } = new Dictionary<string, List<Fragment>>();

        // The final build mapping & summary
        public BuildMapping Recommendation { get; set; } = new BuildMapping();
    }
}
