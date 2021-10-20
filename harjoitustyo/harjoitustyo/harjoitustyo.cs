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
    private string[] alkuValikko = { "Aloita", "Asetukset", "Lopeta peli" };
    private string[] pauseMenu = { "Jatka", "Asetukset", "Palaa aloitusvalikkoon" };
    private string[] kenttaLapaistyMenu = { "Seuraava kenttä", "Palaa aloitusvalikkoon" };
    private string[] peliLapiMenu = { "Aloita alusta", "Lopeta peli" };
    private PhysicsObject pelaaja, maali;
    private AssaultRifle pelaajanAse;
    private Vector nopeusYlos = new Vector(0, nopeus);
    private Vector nopeusVasemmalle = new Vector(-nopeus, 0);
    private Vector nopeusAlas = new Vector(0, -nopeus);
    private Vector nopeusOikealle = new Vector(nopeus, 0);
    public override void Begin()
    {
        ClearAll();
        kenttaNro = 1;
        MultiSelectWindow alkuValikkoV = new MultiSelectWindow("Dungeon Rush", alkuValikko);
        alkuValikkoV.DefaultCancel = -1;
        Add(alkuValikkoV);
        alkuValikkoV.AddItemHandler(0, SeuraavaKentta); // TODO: luo asetukset valikko ja taustakuva alkuvalikolle
        // alkuValikkoV.AddItemHandler(1, AsetusMenu);
        alkuValikkoV.AddItemHandler(2, Exit);
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

    private void AsetusMenu()
    {
        
    }

    private void LuoKentta(int a)
    {
        if (a == 1)
        {
            Level.Background.CreateGradient(Color.BloodRed, Color.SkyBlue); // TODO: luo taustakuva huoneen lattiaksi
            LuoMaali(Level.Left + 50, 0);
            // maali.Oscillate(Vector.UnitY, 100, 0.2);
        }
        else if (a == 2)
        {
            Level.Background.CreateGradient(Color.Orange, Color.BrightGreen);
            LuoMaali(0, Level.Top - 50);
            // maali.Oscillate(Vector.UnitX, 200, 0.4);
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
        maali.MakeStatic();
        Add(maali);
    }

    private void LuoPelaaja()
    {
        Keyboard.Listen(Key.Enter, ButtonState.Pressed, Seuraava, "seuraava kenttä");

        pelaaja = new PhysicsObject(50, 50, Shape.Rectangle); // TODO: luo pelaajalle oma sprite
        pelaaja.Color = Color.DarkBlue;
        Add(pelaaja);

        pelaajanAse = new AssaultRifle(0, 0); // TODO: luo aseelle uusi skin ja ehkä ääniefekti tai poista eseen sprite ja ammu tulipalloja pelaajasta
        pelaajanAse.Ammo.Value = 1;
        pelaajanAse.FireRate = 10;
        pelaajanAse.ProjectileCollision = AmmusOsui;
        pelaajanAse.Image = null;
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
        Keyboard.Listen(Key.Left, ButtonState.Pressed, Ammu, "ampuu vasemmalle", pelaajanAse, nopeusVasemmalle);
        Keyboard.Listen(Key.Right, ButtonState.Pressed, Ammu, "ampuu oikealle", pelaajanAse, nopeusOikealle);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Ammu, "ampuu ylös", pelaajanAse, nopeusYlos);
        Keyboard.Listen(Key.Down, ButtonState.Pressed, Ammu, "ampuu alas", pelaajanAse, nopeusAlas);

        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, PauseValikko, "Avaa valikko");
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
            if (kenttaNro == 3)
            {
                MultiSelectWindow peliLapi = new MultiSelectWindow("Viimeinen kenttä läpäisty", peliLapiMenu);
                Add(peliLapi);
                peliLapi.AddItemHandler(0, Begin);
                peliLapi.AddItemHandler(1, Exit);
                peliLapi.DefaultCancel = -1;
            }
            else
            {
                MultiSelectWindow kenttaLapi = new MultiSelectWindow("Kenttä läpäisty", kenttaLapaistyMenu);
                Add(kenttaLapi);
                kenttaLapi.AddItemHandler(0, Seuraava);
                kenttaLapi.AddItemHandler(1, Begin);
                kenttaLapi.DefaultCancel = -1;
            }
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

    private void Ammu(AssaultRifle ase, Vector suunta)
    {
        pelaajanAse.Angle = suunta.Angle;
        PhysicsObject ammus = ase.Shoot(); // TODO: luo ammukselle uusi skin
        if (ammus != null)
        {
            ammus.Size *= 3;
        }
    }


    private void PauseValikko()
    {
        IsPaused = true;
        MultiSelectWindow pauseMenuV = new MultiSelectWindow("Peli pysäytetty", pauseMenu);
        pauseMenuV.Color = Color.DarkRed; pauseMenuV.SetButtonColor(Color.Black); pauseMenuV.SetButtonTextColor(Color.Red);
        Add(pauseMenuV);
        pauseMenuV.AddItemHandler(0, delegate { Remove(pauseMenuV); IsPaused = false;});
        // pauseMenuV.AddItemHandler(1, AsetusMenu);
        pauseMenuV.AddItemHandler(2, Begin);
    }

    private void Seuraava()
    {
        kenttaNro++;
        SeuraavaKentta();
    }
}

