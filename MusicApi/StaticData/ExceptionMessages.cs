namespace MusicApi.StaticData
{
    public static class ExceptionMessages
    {
        // Message for invalid entity data, which maybe fields are null or bad format.
        public static string InvalidEntityData = "Invalid entity data, check format and value for required fields.";
        // Message for an invalid entity ID (create -> id=0, update/del -> id!=0).
        public static string InvalidEntityId = "Invalid ID, wanted format (create -> id=0, update/del -> id>0).";
        // Message for when an entity doesn't exist for update/delete operations.
        public static string EntityDoesntExist = "Check ID, can't find entity to update/delete.";
    }
}