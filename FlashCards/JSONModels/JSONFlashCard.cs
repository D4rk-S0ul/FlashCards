﻿using FlashCards.ViewModels;

namespace FlashCards.JSONModels;

public class JSONFlashCard
{
    public FlashCardSide Front { get; set; }
    public FlashCardSide Back { get; set; }

    public JSONFlashCard(Layouts frontLayout, bool frontShowBulletPointsIndividually, Layouts backLayout, bool backShowBulletPointsIndividually)
    {
        Front = new FlashCardSide { Layout = frontLayout, ShowBulletPointsIndividually = frontShowBulletPointsIndividually };
        Back = new FlashCardSide { Layout = backLayout, ShowBulletPointsIndividually = backShowBulletPointsIndividually };
    }
}

public class FlashCardSide
{
    public Layouts Layout { get; set; }
    public bool ShowBulletPointsIndividually { get; set; }
}
