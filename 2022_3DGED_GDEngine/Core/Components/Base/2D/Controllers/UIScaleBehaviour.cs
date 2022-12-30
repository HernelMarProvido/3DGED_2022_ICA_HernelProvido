using GD.Engine.Events;
using Microsoft.Xna.Framework;

namespace GD.Engine
{
    public class UIScaleBehaviour : Component
    {
        #region Fields

        private float timeToLiveMs = 2000;
        private float scaleBy;

        #endregion Fields

        //temps
        private float elapsedTimeSince;

        #region Constructors

        public UIScaleBehaviour(float timeToLiveMs, float scaleBy)
        {
            this.timeToLiveMs = timeToLiveMs;
            this.scaleBy = scaleBy;
        }

        #endregion Constructors

        public override void Update(GameTime gameTime)
        {
            elapsedTimeSince += gameTime.ElapsedGameTime.Milliseconds;

            //scale change
            transform.ScaleBy(scaleBy);

            if (elapsedTimeSince >= timeToLiveMs)
            {
                object[] parameters = { this };
                EventDispatcher.Raise(new EventData(EventCategoryType.UiObject, EventActionType.OnRemoveObject, parameters));
            }
        }
    }
}