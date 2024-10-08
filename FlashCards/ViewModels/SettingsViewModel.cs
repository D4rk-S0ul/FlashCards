﻿using CommunityToolkit.Mvvm.ComponentModel;
using FlashCards.Contracts.Services;
using FlashCards.DBModels;
using FlashCards.Helpers;
using System.Collections.ObjectModel;
using System.Reflection;
using Windows.ApplicationModel;

namespace FlashCards.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;

    private readonly IDemotionSettingsService _demotionSettingsService;

    private readonly IDatabaseService _databaseService;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private string _selectedTheme;

    [ObservableProperty]
    private string _selectedLanguageTag;

    [ObservableProperty]
    private string _selectedDemotionTag;

    [ObservableProperty]
    private ObservableCollection<VMBox> _boxes;

    [ObservableProperty]
    private ObservableCollection<VMSubject> _subjects;

    [ObservableProperty]
    private ObservableCollection<VMTag> _tags;

    public SettingsViewModel(IThemeSelectorService themeSelectorService, IDemotionSettingsService demotionSettingsService, IDatabaseService databaseService)
    {
        _themeSelectorService = themeSelectorService;
        _demotionSettingsService = demotionSettingsService;
        _databaseService = databaseService;
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

    private ObservableCollection<VMBox> GetBoxes()
    {
        ObservableCollection<VMBox> boxes = new(_databaseService.GetBoxes().Select(box => new VMBox(box.Id, box.Number, (int)box.DueAfter)));
        return boxes;
    }

    internal void AddBox(int id, int number, DueAfterOptions dueAfter)
    {
        Boxes.Add(new VMBox(id, number, (int)dueAfter));
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

    private ObservableCollection<VMSubject> GetSubjects()
    {
        ObservableCollection<VMSubject> subjects = new(_databaseService.GetSubjects().Select(subject => new VMSubject(subject.Id, subject.Name)));
        return subjects;
    }

    internal void AddSubject(int id, string name)
    {
        Subjects.Add(new VMSubject(id, name));
        SetSubjectEditingState(id, true);
    }

    internal void EditSubject(int id, string name)
    {
        var subject = Subjects.FirstOrDefault(subject => subject.SubjectID == id);
        if (subject is null)
        {
            return;
        }

        int index = Subjects.IndexOf(subject);
        subject.SubjectName = name;
        Subjects[index] = subject;
    }

    internal void SetSubjectEditingState(int id, bool editingState)
    {
        var subject = Subjects.FirstOrDefault(subject => subject.SubjectID == id);
        if (subject is null)
        {
            return;
        }

        int index = Subjects.IndexOf(subject);
        subject.EditingState = editingState;
        Subjects[index] = subject;
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

    private ObservableCollection<VMTag> GetTags()
    {
        ObservableCollection<VMTag> tags = new(_databaseService.GetTags().Select(tag => new VMTag(tag.Id, tag.Name)));
        return tags;
    }

    internal void AddTag(int id, string name)
    {
        Tags.Add(new VMTag(id, name));
        SetTagEditingState(id, true);
    }

    internal void EditTag(int id, string name)
    {
        var tag = Tags.FirstOrDefault(tag => tag.TagID == id);
        if (tag is null)
        {
            return;
        }

        int index = Tags.IndexOf(tag);
        tag.TagName = name;
        Tags[index] = tag;
    }

    internal void SetTagEditingState(int id, bool editingState)
    {
        var tag = Tags.FirstOrDefault(tag => tag.TagID == id);
        if (tag is null)
        {
            return;
        }

        int index = Tags.IndexOf(tag);
        tag.EditingState = editingState;
        Tags[index] = tag;
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
