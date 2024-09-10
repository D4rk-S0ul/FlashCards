﻿using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

using FlashCards.Contracts.Services;
using FlashCards.DBModels;
using FlashCards.Helpers;

using Windows.ApplicationModel;

namespace FlashCards.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    private readonly IDemotionSettingsService _demotionSettingsService;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private string _selectedTheme;

    [ObservableProperty]
    private string _selectedLanguageTag;

    [ObservableProperty]
    private string _selectedDemotionTag;

    [ObservableProperty]
    private ObservableCollection<Box> _boxes;

    [ObservableProperty]
    private ObservableCollection<Subject> _subjects;

    [ObservableProperty]
    private ObservableCollection<Tag> _tags;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IDemotionSettingsService demotionSettingsService)
    {
        _themeSelectorService = themeSelectorService;
        _demotionSettingsService = demotionSettingsService;
        _versionDescription = GetVersionDescription();
        _selectedTheme = _themeSelectorService.Theme.ToString();
        _selectedLanguageTag = GetSelectedLanguageTag();
        _selectedDemotionTag = _demotionSettingsService.SelectedDemotionTag;
        _boxes = GetBoxes();
        _subjects = GetSubjects();
        _tags = GetTags();
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        var appDisplayName = WinUI3Localizer.Localizer.Get().GetLocalizedString("AppDisplayName");
        return $"{appDisplayName} - {version.Major}.{version.Minor}.{version.Build}";
    }

    private static string GetSelectedLanguageTag()
    {
        return WinUI3Localizer.Localizer.Get().GetCurrentLanguage();
    }

    private static ObservableCollection<Box> GetBoxes()
    {
        IDatabaseService databaseService = App.GetService<IDatabaseService>();
        ObservableCollection<Box> boxes = new(databaseService.GetBoxes().Select(box => new Box(box.Id, box.Number, (int)box.DueAfter)));
        return boxes;
    }

    internal void AddBox(int id, int number, DueAfterOptions dueAfter)
    {
        Boxes.Add(new Box(id, number, (int)dueAfter ));
    }

    internal void DeleteBox(int id)
    {
        var box = Boxes.FirstOrDefault(box => box.BoxID == id);
        if (box is null)
        {
            return;
        }
        int boxNumber = int.Parse(box.BoxName.Split(" ")[1]);
        Boxes.Remove(box);
    }

    private static ObservableCollection<Subject> GetSubjects()
    {
        IDatabaseService databaseService = App.GetService<IDatabaseService>();
        ObservableCollection<Subject> subjects = new(databaseService.GetSubjects().Select(subject => new Subject(subject.Id, subject.Name)));
        return subjects;
    }

    internal void AddSubject(int id, string name)
    {
        Subjects.Add(new Subject(id, name));
    }

    internal void DeleteSubject(int id)
    {
        var subject = Subjects.FirstOrDefault(subject => subject.SubjectID == id);
        if (subject is null)
        {
            return;
        }
        Subjects.Remove(subject);
    }

    private static ObservableCollection<Tag> GetTags()
    {
        IDatabaseService databaseService = App.GetService<IDatabaseService>();
        ObservableCollection<Tag> tags = new(databaseService.GetTags().Select(tag => new Tag(tag.Id, tag.Name)));
        return tags;
    }

    internal void AddTag(int id, string name)
    {
        Tags.Add(new Tag(id, name));
    }

    internal void DeleteTag(int id)
    {
        var tag = Tags.FirstOrDefault(tag => tag.TagID == id);
        if (tag is null)
        {
            return;
        }
        Tags.Remove(tag);
    }
}

public class Box(int boxID, int number, int selectedIndex)
{
    public int BoxID { get; set; } = boxID;
    public string BoxName { get; set; } = $"{WinUI3Localizer.Localizer.Get().GetLocalizedString("Box")} {number}";
    public int SelectedIndex { get; set; } = selectedIndex;
}

public class Subject(int subjectID, string subjectName)
{
    public int SubjectID { get; set; } = subjectID;
    public string SubjectName { get; set; } = subjectName;
}

public class Tag(int tagID, string tagName)
{
    public int TagID { get; set; } = tagID;
    public string TagName { get; set; } = tagName;
}

