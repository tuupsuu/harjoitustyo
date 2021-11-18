using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class harjoitustyo : PhysicsGame
{
    private IntMeter vihollistenMaara, elamaMittari;
    private int kenttaNro = 1;
    private static double nopeus = 100;
    private string[] alkuValikko = { "Aloita", "Lopeta peli" }, pauseMenu = { "Jatka", "Palaa aloitusvalikkoon" }, kenttaLapaistyMenu = { "Seuraava kenttä", "Palaa aloitusvalikkoon" }, peliLapiMenu = { "Aloita alusta", "Lopeta peli" };
    private PhysicsObject pelaaja, maali;
    private AssaultRifle pelaajanAse;
    private Vector nopeusYlos = new Vector(0, nopeus), nopeusVasemmalle = new Vector(-nopeus, 0), nopeusAlas = new Vector(0, -nopeus), nopeusOikealle = new Vector(nopeus, 0);
    public override void Begin()
    {
        ClearAll();
        kenttaNro = 1;
        MultiSelectWindow alkuValikkoV = new MultiSelectWindow("Dungeon Rush", alkuValikko);
        alkuValikkoV.DefaultCancel = -1;
        Add(alkuValikkoV);
        alkuValikkoV.AddItemHandler(0, SeuraavaKentta);
        alkuValikkoV.AddItemHandler(1, Exit);
    }


    private void SeuraavaKentta()
    {
        ClearAll();
        LuoPistelaskuri();

        if (kenttaNro == 1) LuoKentta(1);
        else if (kenttaNro == 2) LuoKentta(2);
        else if (kenttaNro == 3) LuoKentta(3);
        else if (kenttaNro > 3) Exit();

    }


    private void LuoPistelaskuri()
    {
        {
            vihollistenMaara = new IntMeter(0);
            Label laskuri = new Label();
            laskuri.Title = "Vihollisia jäljellä: ";
            laskuri.X = 0;
            laskuri.Y = 350;
            laskuri.TextColor = Color.Black;
            laskuri.Color = Color.White;
            laskuri.BindTo(vihollistenMaara);
            Add(laskuri);
        }
        {
            elamaMittari = new IntMeter(3);
            elamaMittari.MaxValue = 3;
            elamaMittari.LowerLimit += ElamaLoppui;
            ProgressBar elamaPalkki = new ProgressBar(150, 20);
            elamaPalkki.X = -400;
            elamaPalkki.Y = 350;
            elamaPalkki.BindTo(elamaMittari);
            Add(elamaPalkki);
        }
    }


    private void LuoKentta(int a)
    {
        if (a == 1)
        {
            Level.Background.CreateGradient(Color.BloodRed, Color.SkyBlue);
            vihollistenMaara.Value = 0;
            {
                TileMap kentta11 = TileMap.FromLevelAsset("kentta_1_1");
                kentta11.SetTileMethod('M', LuoMaali);
                kentta11.SetTileMethod('P', LuoPelaaja);
                kentta11.SetTileMethod('s', LuoSeina);
                kentta11.Execute(30, 40);
            }
            {
                TileMap kentta11 = TileMap.FromLevelAsset("kentta_1_1");
                kentta11.SetTileMethod('N', LuoVihu, 0);
                kentta11.SetTileMethod('F', LuoVihu, 1);
                kentta11.SetTileMethod('L', LuoVihu, 2);
                kentta11.Execute(30, 40);
            }


        }
        else if (a == 2)
        {
            Level.Background.CreateGradient(Color.Orange, Color.BrightGreen);
            vihollistenMaara.Value = 0;
            {
                TileMap kentta11 = TileMap.FromLevelAsset("harjoitusKentta");
                kentta11.SetTileMethod('M', LuoMaali);
                kentta11.SetTileMethod('P', LuoPelaaja);
                kentta11.SetTileMethod('s', LuoSeina);
                kentta11.Execute(30, 40);
            }
            {
                TileMap kentta11 = TileMap.FromLevelAsset("harjoitusKentta");
                kentta11.SetTileMethod('N', LuoVihu, 0);
                kentta11.SetTileMethod('F', LuoVihu, 1);
                kentta11.SetTileMethod('L', LuoVihu, 2);
                kentta11.Execute(30, 40);
            }
        }
        else if (a == 3)
        {
            Level.Background.CreateGradient(Color.White, Color.Black);
            vihollistenMaara.Value = 0;
            {
                TileMap kentta11 = TileMap.FromLevelAsset("kentta_1_1");
                kentta11.SetTileMethod('M', LuoMaali);
                kentta11.SetTileMethod('P', LuoPelaaja);
                kentta11.SetTileMethod('s', LuoSeina);
                kentta11.Execute(30, 40);
            }
            {
                TileMap kentta11 = TileMap.FromLevelAsset("kentta_1_1");
                kentta11.SetTileMethod('N', LuoVihu, 0);
                kentta11.SetTileMethod('F', LuoVihu, 1);
                kentta11.SetTileMethod('L', LuoVihu, 2);
                kentta11.Execute(30, 40);
            }
        }

        Level.CreateBorders();
        Camera.ZoomToLevel();
    }

    private void LuoMaali(Vector paikka, double leveys, double korkeus)
    {
        maali = PhysicsObject.CreateStaticObject(leveys, korkeus);
        maali.Position = paikka;
        maali.Tag = "maali";
        maali.CanRotate = false;
        maali.Color = Color.DarkGray;

        Add(maali);
    }

    private void LuoPelaaja(Vector paikka, double leveys, double korkeus)
    {
        Keyboard.Listen(Key.Enter, ButtonState.Pressed, Seuraava, "seuraava kenttä");

        pelaaja = new PhysicsObject(leveys * 0.75, korkeus * 0.75, Shape.Rectangle);
        pelaaja.Position = paikka;
        pelaaja.Color = Color.DarkBlue;
        pelaaja.CanRotate = false;
        pelaaja.Tag = "pelaaja";
        Add(pelaaja);

        pelaajanAse = new AssaultRifle(0, 0);
        pelaajanAse.Ammo.Value = 1;
        pelaajanAse.FireRate = 1;
        pelaajanAse.Position = pelaaja.Position;
        pelaaja.Add(pelaajanAse);

        LuoOhjaimet();

        AddCollisionHandler(pelaaja, PelaajaOsui);

        pelaaja.MaxVelocity = 100;
        pelaaja.Mass = 0.01;
    }



    private void ElamaLoppui()
    {
        List<GameObject> objektit = GetAllObjects();
        for (int i = 0; i < objektit.Count; i++)
        {
            Explosion rajahdys = new Explosion(100);
            rajahdys.Position = objektit[i].Position;
            Remove(objektit[i]);
            Add(rajahdys);
        }
        
        Label kuolinIlmoitus = new Label(250, 50, "Kuolit!!!");
        kuolinIlmoitus.Y = 25;
        kuolinIlmoitus.Color = Color.Black;
        kuolinIlmoitus.TextColor = Color.Red;
        kuolinIlmoitus.BorderColor = Color.Red;
        Add(kuolinIlmoitus);
        Label anyKey = new Label(500, 50, "Paina välilyöntiä palataksesi alkuvalikkoon.");
        anyKey.Color = Color.Black;
        anyKey.TextColor = Color.Red;
        anyKey.BorderColor = Color.Red;
        anyKey.Y = -25;
        Add(anyKey);
        Keyboard.Listen(Key.Space, ButtonState.Pressed, Begin, null);
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



    private void LuoSeina(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject seina = PhysicsObject.CreateStaticObject(leveys, korkeus);
        seina.Position = paikka;
        seina.Tag = "seina";
        seina.Color = Color.Black;
        seina.CanRotate = false;
        Add(seina);
    }



    private void LuoVihu(Vector paikka, double leveys, double korkeus, int versio)
    {
        PhysicsObject vihu = new PhysicsObject(leveys, korkeus, Shape.Rectangle);
        if (versio == 0)
        {
            vihu.MakeStatic();
            vihu.Color = Color.Yellow;
        }
        else if (versio == 1)
        {
            vihu = new PhysicsObject(leveys * 0.75, korkeus * 0.75, Shape.Rectangle);
            vihu.Color = Color.Red;
            FollowerBrain aivot = new FollowerBrain("pelaaja");
            aivot.Speed = nopeus;
            aivot.DistanceFar = 200;
            vihu.Brain = aivot;
        }
        else if (versio == 2)
        {
            vihu = new PhysicsObject(leveys * 0.5, korkeus * 0.5, Shape.Rectangle);
            vihu.Color = Color.Brown;
            LabyrinthWandererBrain aivot = new LabyrinthWandererBrain(korkeus, nopeus, "seina");
            vihu.Brain = aivot;
        }
        vihu.CanRotate = false;
        vihollistenMaara.Value += 1;
        vihu.Tag = "vihu";
        vihu.Position = paikka;
        Add(vihu);
    }


    private void AmmusOsui(PhysicsObject ammus, PhysicsObject osuvaKohde)
    {
        if (osuvaKohde.Tag.ToString() == "vihu")
        {
            osuvaKohde.Destroy();
            vihollistenMaara.Value -= 1;
            if (vihollistenMaara == 0) maali.Color = Color.Green;
        }
        ammus.Destroy();
        pelaajanAse.Ammo.Value += 1;
    }



    private void Ammu(AssaultRifle ase, Vector suunta)
    {
        ase.Angle = suunta.Angle;
        PhysicsObject ammus = ase.Shoot();
        if (ammus != null)
        {
            ammus.Size *= 1;
            ammus.Tag = "luoti";
            AddCollisionHandler(ammus, AmmusOsui);
        }
    }

    private void PelaajaOsui(PhysicsObject osuvaKohde, PhysicsObject osuttuKohde)
    {
        if (osuttuKohde.Tag.ToString() == "maali")
        {
            if (!(vihollistenMaara.Value == 0))
            {
                MessageDisplay.Clear();
                MessageDisplay.Add("Tapa ensin kaikki viholliset!");
            }
            else
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
        else if (osuttuKohde.Tag.ToString() == "vihu")
        {
            elamaMittari.Value -= 1;
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


    private void PauseValikko()
    {
        IsPaused = true;
        MultiSelectWindow pauseMenuV = new MultiSelectWindow("Peli pysäytetty", pauseMenu);
        pauseMenuV.Color = Color.DarkRed; pauseMenuV.SetButtonColor(Color.Black); pauseMenuV.SetButtonTextColor(Color.Red);
        Add(pauseMenuV);
        pauseMenuV.AddItemHandler(0, delegate { Remove(pauseMenuV); IsPaused = false;});
        pauseMenuV.AddItemHandler(1, Begin);
    }

    private void Seuraava()
    {
        kenttaNro++;
        SeuraavaKentta();
    }
}

