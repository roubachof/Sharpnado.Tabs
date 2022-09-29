// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SillyVmo.cs" company="The Silly Company">
//   The Silly Company 2016. All rights reserved.
// </copyright>
// <summary>
//   The silly vmo.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;
using MauiSample.Domain.Silly;

namespace MauiSample.Presentation.ViewModels
{
    public class SillyDudeVmo
    {
        public SillyDudeVmo(SillyDude dude, ICommand onItemTappedCommand)
        {
            if (dude != null)
            {
                Id = dude.Id;
                Name = dude.Name;
                FullName = dude.FullName;
                Role = dude.Role;
                Description = dude.Description;
                ImageUrl = dude.ImageUrl;
                SillinessDegree = dude.SillinessDegree;
                SourceUrl = dude.SourceUrl;
            }

            OnItemTappedCommand = onItemTappedCommand;
        }

        public bool IsMovable { get; protected set; } = true;

        public ICommand OnItemTappedCommand { get; set; }

        public int Id { get; }

        public string Name { get; }

        public string FullName { get; }

        public string Role { get; }

        public string Description { get; }

        public string ImageUrl { get; }

        public int SillinessDegree { get; }

        public string SourceUrl { get; }

        public void Lock()
        {
            IsMovable = false;
        }
    }

    public class AddSillyDudeVmo : SillyDudeVmo
    {
        public AddSillyDudeVmo(ICommand onItemTappedCommand)
            : base(null, onItemTappedCommand)
        {
        }
    }
}