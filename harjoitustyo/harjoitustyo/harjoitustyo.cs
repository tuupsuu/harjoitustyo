using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class harjoitustyo : PhysicsGame
{
    private int kenttaNro = 1;
    private static double nopeus = 100;
    private PhysicsObject pelaaja, maali;
    private AssaultRifle pelaajanAse;
    private Vector nopeusYlos = new Vector(0, nopeus);
    private Vector nopeusVasemmalle = new Vector(-nopeus, 0);
    private Vector nopeusAlas = new Vector(0, -nopeus);
    private Vector nopeusOikealle = new Vector(nopeus, 0);
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
            LuoMaali(Level.Left + 50, 0);
            maali.Oscillate(Vector.UnitY, 100, 0.2);
        }
        else if (a == 2)
        {
            Level.Background.CreateGradient(Color.Orange, Color.BrightGreen);
            LuoMaali(0, Level.Top - 50);
            maali.Oscillate(Vector.UnitX, 200, 0.4);
        }
        else if (a == 3)
        {
            Level.Background.CreateGradient(Color.White, Color.Black);
            LuoMaali(Level.Left + 50, Level.Top - 50);
        }

        Level.CreateBorders();
        Camera.ZoomToLevel();
    }

    private void LuoMaali(double x, double y)
    {
        maali = new PhysicsObject(50, 50, Shape.Rectangle, x, y);
        maali.Tag = "maali";
        maali.CanRotate = true;
        maali.AngularVelocity = 100;
        Add(maali);
    }

    private void LuoPelaaja()
    {
        Keyboard.Listen(Key.Space, ButtonState.Pressed, Seuraava, "seuraava kenttä");

        pelaaja = new PhysicsObject(50, 50, Shape.Rectangle); // TODO: luo pelaajalle oma sprite
        pelaaja.Color = Color.DarkBlue;
        Add(pelaaja);

        pelaajanAse = new AssaultRifle(50, 20); // TODO: luo aseelle uusi skin ja ehkä ääniefekti 
        pelaajanAse.Ammo.Value = 1;
        pelaajanAse.FireRate = 10;
        pelaajanAse.ProjectileCollision = AmmusOsui;
        pelaaja.Add(pelaajanAse);

        LuoOhjaimet();

        AddCollisionHandler(pelaaja, Maalissa);

        pelaaja.MaxVelocity = 100;
        pelaaja.Mass = 0.01;
    }

    private void LuoOhjaimet()
    {
        Keyboard.Listen(Key.A, ButtonState.Down, Liiku, "liikuttaa pelaajaa vasemmalle", nopeusVasemmalle);
        Keyboard.Listen(Key.A, ButtonState.Released, Pysayta, null);

        Keyboard.Listen(Key.D, ButtonState.Down, Liiku, "liikuttaa pelaajaa oikealle", nopeusOikealle);
        Keyboard.Listen(Key.D, ButtonState.Released, Pysayta, null);

        Keyboard.Listen(Key.W, ButtonState.Down, Liiku, "liikuttaa pelaajaa ylös", nopeusYlos);
        Keyboard.Listen(Key.W, ButtonState.Released, Pysayta, null);

        Keyboard.Listen(Key.S, ButtonState.Down, Liiku, "liikuttaa pelaajaa alas", nopeusAlas);
        Keyboard.Listen(Key.S, ButtonState.Released, Pysayta, null);
        // yllä pelaajan liikuttaminen, alla pelaajan ampuminen
        Keyboard.Listen(Key.Left, ButtonState.Pressed, Ammu, "ampuu vasemmalle", pelaajanAse);
        Keyboard.Listen(Key.Right, ButtonState.Pressed, Ammu, "ampuu oikealle", pelaajanAse);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Ammu, "ampuu ylös", pelaajanAse);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, Ammu, "ampuu alas", pelaajanAse);

        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    private void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();
        pelaajanAse.Ammo.Value += 1;
    }

    private void Maalissa(PhysicsObject osuvaKohde, PhysicsObject osuttuKohde)
    {
        if (osuttuKohde.Tag.ToString() == "maali")
        {
            Seuraava();
        }
    }

    private void Liiku(Vector nopeus)
    {
        pelaaja.Push(nopeus);
    }

    private void Pysayta()
    {
        pelaaja.Stop();
    }

    private void Ammu(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot(); // TODO: luo ammukselle uusi skin
        if (ammus != null)
        {
            ammus.Size *= 3;
        }
    }


    private void Seuraava()
    {
        kenttaNro++;
        SeuraavaKentta();
    }
}

