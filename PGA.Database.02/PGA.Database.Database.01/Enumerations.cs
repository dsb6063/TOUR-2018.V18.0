namespace PGA.Database
{
    public class Enumerations
    {
        public enum CommandState
        {
            INITIALIZED,
            LOADED,
            INVOKED,
            STARTED,
            FINISHED,
            COMPLETED,
            ERROR
        }

        public enum Function
        {
            LOADING_DWGS,
            COPYING_DWGS,
            INSERTING_BLOCKS,
            CHANGE_LAYERS,
            SIMPLIFY_PLINES,
            SAVING,
            WAITING,
            CREATING_SURFACE,
            DATABASE_INSERT,
            DATABASE_UPDATE,
            DATABASE_DELETE
        }
    }
}