﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;

namespace FlappyNez.Entities
{
    class Rock : Entity
    {
        string _spriteName = string.Empty;
        Sprite _sprite;
        public bool IsUp;
        int _offset;
        Mover _mover;

        public Rock(int offset = 0, bool isUp = true) : base()
        {
            this.IsUp = isUp;
            _offset = offset;

            if (isUp)
            {
                name = "Rock_Down";
                _spriteName = Content.Terrain.rockGrassDown;
            }
            else
            {
                name = "Rock_Up";
                _spriteName = Content.Terrain.rockGrass;
            }

            // Mover component
            _mover = addComponent(new Mover());
        }

        public override void onAddedToScene()
        {
            base.onAddedToScene();

            // Load sprite and add it in entity
            _sprite = addComponent(new Sprite(scene.content.Load<Texture2D>(_spriteName)));

            // RenderLayer to 1 because need drawing rock behind terrain
            _sprite.renderLayer = 1;

            // Polygon Collider
            var collider = addCollider(new PolygonCollider(GetRockVertices()));

            // Must colliders with only layer 0 (in other words with plane only)
            Flags.setFlagExclusive(ref collider.collidesWithLayers, 0);

            ResetPosition();
        }

        private void ResetPosition()
        {
            var _gapHeight = MathHelper.Clamp(Constants.GapHeight, (_sprite.height / 2), _sprite.height);
            var realRockHeight = (Screen.height - _gapHeight) / 2;
            var nominalRockHeight = (_sprite.height / 2) - (_sprite.height - realRockHeight);

            transform.position = new Vector2((Screen.width + (_sprite.width / 2)),
                IsUp ? (_offset + nominalRockHeight) : (_offset + Screen.height - nominalRockHeight));
        }

        private Vector2[] GetRockVertices()
        {
            // TODO: This code is dirty (and hard coded) solution. Rewrite this!
            //       Must extract vertices from Texture2 as Farseer Physics does
            var height = _sprite.height * (IsUp ? -1 : 1);
            var width = _sprite.width * (IsUp ? 1 : -1);

            var verts = new Vector2[4];
            verts[(IsUp ? 1 : 0)] = new Vector2(8, -(height / 2));
            verts[(IsUp ? 0 : 1)] = new Vector2(16, -(height / 2));
            verts[2] = new Vector2(-(width / 2), (height / 2));
            verts[3] = new Vector2((width / 2), (height / 2));

            return verts;
        }

        public override void update()
        {
            base.update();

            CollisionResult res;
            _mover.move(new Vector2(-1, 0) * Constants.ObstaclesSpeed * Time.deltaTime, out res);

            if (transform.position.X <= -(_sprite.width / 2))
            {
                this.destroy();
            }
        }
    }
}
