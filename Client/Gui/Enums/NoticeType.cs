namespace Client.Gui.Enums;

public enum NoticeType
{
    // There are way too many of these.
    NameRestriction = 0,
    DeleteCharacter = 1,
    NotRegistered = 2,
    IncorrectPass = 3,
    IncorrectUser = 4,
    NameTaken = 5, // 8 can also be used
    AvailableName = 6,
    ReturnToLogin = 7,
    NameTaken2 = 8,
    CharSlotsFilled = 9,
}

public enum NoticeBackground
{
    Information = 0,
    Error = 1,
    EnterText = 2,
}