using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Breakout
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Breakout : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Texture2D blockTexture;
		Texture2D pixelTexture;
		Texture2D ballTexture;
		Texture2D winMessage;
		List<Rectangle> blocks;
		Rectangle paddle;
		Rectangle ball;
		bool showWinMessage = false;

		Vector2 ballSpeed;

		bool gameRunning;

		public Breakout()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			ResetLevel();

			base.Initialize();
		}

		protected void ResetLevel()
		{
			int blockRows = 5;
			int blockCols = 15;
			Rectangle blockSize = new Rectangle(0, 0, 50, 20);
			blocks = new List<Rectangle>();

			for (int cols = 0; cols < blockCols; cols++)
			{
				for (int rows = 0; rows < blockRows; rows++)
				{
					blocks.Add(new Rectangle(20 + cols * blockSize.Width, 20 + rows * blockSize.Height, blockSize.Width, blockSize.Height));
				}
			}

			this.ResetPaddle();
		}

		protected void ResetPaddle()
		{

			paddle = new Rectangle(0, 0, 50, 10);
			paddle.X = graphics.PreferredBackBufferWidth / 2 - paddle.Width / 2;
			paddle.Y = graphics.PreferredBackBufferHeight - 20;

			ball = new Rectangle(paddle.Center.X, paddle.Center.Y - paddle.Height - 5, 10, 10);
			ballSpeed = Vector2.Zero;

			gameRunning = false;
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			blockTexture = Content.Load<Texture2D>("block");
			ballTexture = Content.Load<Texture2D>("ball");
			winMessage = Content.Load<Texture2D>("winMessage");

			pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
			pixelTexture.SetData<Color>(new Color[] { Color.White });
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			var mouseState = Mouse.GetState();

			paddle.X = mouseState.X;
			if (paddle.X < 0)
				paddle.X = 0;
			if (paddle.X > graphics.PreferredBackBufferWidth - paddle.Width)
				paddle.X = graphics.PreferredBackBufferWidth - paddle.Width;

			if (!gameRunning)
			{
				ball.X = paddle.Center.X - 5;

				if (mouseState.LeftButton == ButtonState.Pressed)
				{
					gameRunning = true;
					ballSpeed.Y = -5;
					showWinMessage = false;
				}
			}

			ball.X += (int)ballSpeed.X;
			ball.Y += (int)ballSpeed.Y;

			if (ball.Intersects(paddle))
			{
				ballSpeed.Y *= -1;
				ballSpeed.X = (ball.Center.X - paddle.Center.X) / 3;
			}

			foreach (var block in blocks)
			{
				if (ball.Intersects(block))
				{
					blocks.Remove(block);
					ballSpeed.Y *= -1;
					break;
				}
			}

			if (blocks.Count() == 0)
			{
				showWinMessage = true;
				ResetLevel();
				return;
			}

			// Wall bounces
			if (ball.Y < 0)
			{
				ballSpeed.Y *= -1;
			}
			if (ball.X < 0 || ball.X > graphics.PreferredBackBufferWidth - ball.Width)
			{
				ballSpeed.X *= -1;
			}

			if (ball.Y > graphics.PreferredBackBufferHeight)
			{
				ResetPaddle();
				return;
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			foreach (var block in blocks)
				spriteBatch.Draw(blockTexture, block, Color.White);

			spriteBatch.Draw(pixelTexture, paddle, Color.White);

			spriteBatch.Draw(ballTexture, ball, Color.White);

			if (showWinMessage)
				spriteBatch.Draw(winMessage, new Vector2(0, -50), Color.White);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
