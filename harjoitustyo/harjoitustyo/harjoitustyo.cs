using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class harjoitustyo : PhysicsGame
{
    int kenttaNro = 1;
    PhysicsObject pelaaja;
    Vector nopeusYlos = new Vector(0, 100);
    Vector nopeusVasemmalle = new Vector(-100, 0);
    Vector nopeusAlas = new Vector(0, -100);
    Vector nopeusOikealle = new Vector(100, 0);
    int juoksuKerroin = 2;
    public override void Begin()
    {
        // Kirjoita ohjelmakoodisi tähän
        SeuraavaKentta();

    }


    private void SeuraavaKentta()
    {
        ClearAll();
        LuoPelaaja();

        if (kenttaNro == 1) LuoKentta(1);
        else if (kenttaNro == 2) LuoKentta(2);
        else if (kenttaNro == 3) LuoKentta(3);
        else if (kenttaNro > 3) Exit();

    }

    private void LuoKentta(int a)
    {
        if (a == 1)
        {
            Level.Background.CreateGradient(Color.BloodRed, Color.SkyBlue); // TODO: luo taustakuva huoneen lattiaksi
        }
        else if (a == 2)
        {
            Level.Background.CreateGradient(Color.Orange, Color.BrightGreen);
        }
        else if (a == 3)
        {
            Level.Background.CreateGradient(Color.White, Color.Black);
        }
    }

    private void LuoPelaaja()
    {
        Keyboard.Listen(Key.Space, ButtonState.Pressed, Seuraava, "seuraava kenttä");

        pelaaja = new PhysicsObject(50, 50, Shape.Rectangle); // TODO: luo pelaajalle oma sprite
        pelaaja.Color = Color.DarkBlue;
        Add(pelaaja);

        Keyboard.Listen(Key.A, ButtonState.Down, Liiku, "liikuttaa pelaajaa vasemmalle", pelaaja, nopeusVasemmalle);
        Keyboard.Listen(Key.A, ButtonState.Released, Liiku, "pysäyttää pelaajan liikkeen", pelaaja, Vector.Zero);

        Keyboard.Listen(Key.D, ButtonState.Down, Liiku, "liikuttaa pelaajaa oikealle", pelaaja, nopeusOikealle);
        Keyboard.Listen(Key.D, ButtonState.Released, Liiku, "pysäyttää pelaajan liikkeen", pelaaja, Vector.Zero);

        Keyboard.Listen(Key.W, ButtonState.Down, Liiku, "liikuttaa pelaajaa ylös", pelaaja, nopeusYlos);
        Keyboard.Listen(Key.W, ButtonState.Released, Liiku, "pysäyttää pelaajan liikkeen", pelaaja, Vector.Zero);

        Keyboard.Listen(Key.S, ButtonState.Down, Liiku, "liikuttaa pelaajaa alas", pelaaja, nopeusAlas);
        Keyboard.Listen(Key.S, ButtonState.Released, Liiku, "pysäyttää pelaajan liikkeen", pelaaja, Vector.Zero);

        // Keyboard.Listen(Key.LeftShift, ButtonState.Pressed, Syoksy, "pelaaja juoksee painettaessa alas", pelaaja, juoksuKerroin);
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    private void Liiku(PhysicsObject pelaaja, Vector nopeus)
    {
        pelaaja.Velocity = nopeus;
        if (Keyboard.IsShiftDown()) pelaaja.Velocity = pelaaja.Velocity * 2;
    }
    /*
    private void Syoksy(PhysicsObject physics, int kerroin)
    {
        if (Keyboard.Listen(Key.W, ButtonState.Down))
        pelaaja.Hit()
    } */


    private void Seuraava()
    {
        kenttaNro++;
        SeuraavaKentta();
    }
}

