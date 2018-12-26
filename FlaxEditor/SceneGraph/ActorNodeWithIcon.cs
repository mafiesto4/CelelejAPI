// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using FlaxEngine;

namespace FlaxEditor.SceneGraph
{
    /// <summary>
    /// Helper class for actors with icon drawn in editor (eg. lights, probes, etc.).
    /// </summary>
    /// <seealso cref="ActorNode" />
    public abstract class ActorNodeWithIcon : ActorNode
    {
        /// <inheritdoc />
        protected ActorNodeWithIcon(Actor actor)
        : base(actor)
        {
        }

        /// <inheritdoc />
        public override bool RayCastSelf(ref RayCastData ray, out float distance)
        {
            // Check if skip raycasts
            if ((ray.Flags & RayCastData.FlagTypes.SkipEditorPrimitives) == RayCastData.FlagTypes.SkipEditorPrimitives)
            {
                distance = 0;
                return false;
            }

            BoundingSphere sphere = new BoundingSphere(Transform.Translation, 7.0f);
            return CollisionsHelper.RayIntersectsSphere(ref ray.Ray, ref sphere, out distance);
        }
    }
}
