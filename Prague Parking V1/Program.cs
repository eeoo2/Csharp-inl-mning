string[] parkeringen = new string[100];

bool körMeny = true;

while (körMeny)
{
    Console.WriteLine("\nPARKERINGSMENY\n");
    Console.WriteLine("1. Ta emot fordon");
    Console.WriteLine("2. Sök efter fordon");
    Console.WriteLine("3. Flytta fordon");
    Console.WriteLine("4. Uthämtning/borttagning av fordon");
    Console.WriteLine("5. Avsluta");
    Console.Write("\nVälj ett alternativ: ");


    if (int.TryParse(Console.ReadLine(), out int alternativ))
    {
        Console.Clear();
        switch (alternativ)
        {
            case 1:
                {
                    int fordonsVal = 0;
                    while (fordonsVal < 1 || fordonsVal > 3)
                    {
                        Console.WriteLine("Vad vill du parkera?\n");
                        Console.WriteLine("1. Bil");
                        Console.WriteLine("2. MC");
                        Console.WriteLine("3. Tillbaka till menyn");
                        Console.Write("\nVälj ett alternativ: ");
                        if (!int.TryParse(Console.ReadLine(), out fordonsVal) || fordonsVal < 1 || fordonsVal > 3)
                        {
                            Console.WriteLine("\nOgiltigt val, försök igen.\n");
                        }
                    }

                    if (fordonsVal == 3)
                    {
                        break;
                    }

                    string fordonstyp = "bil";
                    if (fordonsVal == 2)
                    {
                        fordonstyp = "mc";
                    }

                    Console.WriteLine("\nAnge registreringsnummer: ");
                    string regNr = Console.ReadLine().ToUpper();
                    while (string.IsNullOrWhiteSpace(regNr) || regNr.Length > 10)
                    {
                        Console.WriteLine("\nOgiltigt registreringsnummer, försök igen.");
                        Console.WriteLine("\nAnge registreringsnummer: ");
                        regNr = Console.ReadLine().ToUpper();
                    }
                    Console.WriteLine($"\nFordonstyp: {fordonstyp}. Registreringsnummer: {regNr}");

                    if (fordonstyp.ToLower() == "mc")
                    {
                        int mcParkering = MCPlats(parkeringen);
                        if (mcParkering != -1)
                        {
                            //Lägg till en andra MC på samma plats, separerad med "|".
                            parkeringen[mcParkering - 1] = parkeringen[mcParkering - 1] + "|" + "MC#" + regNr;
                            Console.WriteLine($"\nMC {regNr} parkerad på plats {mcParkering}");
                            break;
                        }
                    }
                    int plats = LedigPlats(parkeringen);
                    if (plats == -1)
                    {
                        Console.WriteLine("\nParkeringen är full");
                    }
                    else
                    {
                        string typ = "MC";
                        if (fordonstyp.ToLower() == "bil")
                        {
                            typ = "CAR";
                        }

                        parkeringen[plats - 1] = typ + "#" + regNr;
                        Console.WriteLine($"\n{fordonstyp.ToUpper()}-{regNr} parkerad på Plats {plats}");
                    }

                    break;
                }

            case 2:
                {
                    Console.WriteLine("Sök efter fordon, ange registreringsnummer: ");
                    string sökRegNr = Console.ReadLine().ToUpper();
                    int hittatFordon = SökFordon(parkeringen, sökRegNr);

                    if (hittatFordon != -1)
                    {
                        Console.WriteLine($"\nFordonet med registreringsnummer {sökRegNr} finns parkerad på Plats {hittatFordon}");
                    }
                    else
                    {
                        Console.WriteLine("\nFordonet kunde inte hittas");
                    }

                    break;
                }

            case 3:
                {
                    Console.WriteLine("Ange registreringsnumret på det fordon du vill flytta: ");
                    string sökRegNr = Console.ReadLine().ToUpper();
                    int hittatFordon = SökFordon(parkeringen, sökRegNr);
                    if (hittatFordon != -1)
                    {
                        Console.WriteLine($"\nFordonet med registreringsnummer {sökRegNr} finns parkerad på Plats {hittatFordon}");
                        Console.WriteLine();
                        VisaParkering(parkeringen);
                        Console.WriteLine();

                        //Dela upp två MC på samma plats så att rätt fordon kan väljas.
                        string[] fordonPåPlats = parkeringen[hittatFordon - 1].Split('|');
                        string valtFordon = "";
                        foreach (var f in fordonPåPlats)
                        {
                            if (f.Contains(sökRegNr))
                            {
                                valtFordon = f;
                            }
                        }

                        while (true)
                        {
                            Console.WriteLine("\nTill vilken plats vill du flytta fordonet? (Tryck 0 för att gå tillbaka till menyn)");
                            int.TryParse(Console.ReadLine(), out int nyPlats);

                            if (nyPlats == 0)
                            {
                                break;
                            }

                            string nyPlatsInnehåll = null; // null används tills en giltig parkeringsplats har hittats
                            if (nyPlats >= 1 && nyPlats <= 100)
                            {
                                if (ÄrLedig(parkeringen, nyPlats))
                                {
                                    nyPlatsInnehåll = valtFordon;
                                }
                                else if (valtFordon.StartsWith("MC#") && nyPlats != hittatFordon
                                    && parkeringen[nyPlats - 1].StartsWith("MC#") && !parkeringen[nyPlats - 1].Contains("|"))
                                {
                                    nyPlatsInnehåll = parkeringen[nyPlats - 1] + "|" + valtFordon;
                                }
                            }

                            if (nyPlatsInnehåll != null)
                            {
                                //Töm gamla platsen, eller behåll den MC som inte flyttats
                                if (fordonPåPlats.Length == 1)
                                {
                                    parkeringen[hittatFordon - 1] = null;
                                }
                                else if (fordonPåPlats[0] == valtFordon)
                                {
                                    parkeringen[hittatFordon - 1] = fordonPåPlats[1];
                                }
                                else
                                {
                                    parkeringen[hittatFordon - 1] = fordonPåPlats[0];
                                }

                                parkeringen[nyPlats - 1] = nyPlatsInnehåll; //Spara fordonet (eller båda MC) på den nya platsen

                                Console.WriteLine($"\nFordonet med registreringssnummer {sökRegNr} har flyttats till Plats {nyPlats}");
                                Console.WriteLine();
                                VisaParkering(parkeringen);
                                Console.WriteLine();
                                break;
                            }
                            else
                            {
                                Console.WriteLine("\nPlatsen är upptagen eller ogiltig, försök igen.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nFordonet kunde inte hittas.");
                    }

                    break;
                }

            case 4:
                {
                    Console.WriteLine("Ange registreringsnumret på det fordon du vill hämta/ ta bort: ");
                    string sökRegNr = Console.ReadLine().ToUpper();
                    int hittatFordon = SökFordon(parkeringen, sökRegNr);

                    if (hittatFordon != -1)
                    {
                        Console.WriteLine($"\nFordonet med registreringsnummer {sökRegNr} finns parkerad på Plats {hittatFordon}");

                        Console.WriteLine();
                        VisaParkering(parkeringen);
                        Console.WriteLine();

                        string[] fordonPåPlats = parkeringen[hittatFordon - 1].Split('|');
                        string valtFordon = "";
                        foreach (var f in fordonPåPlats)
                        {
                            if (f.Contains(sökRegNr))
                            {
                                valtFordon = f;
                            }
                        }

                        if (fordonPåPlats.Length == 1)
                        {
                            parkeringen[hittatFordon - 1] = null;
                        }
                        else if (fordonPåPlats[0] == valtFordon)
                        {
                            parkeringen[hittatFordon - 1] = fordonPåPlats[1];
                        }
                        else
                        {
                            parkeringen[hittatFordon - 1] = fordonPåPlats[0];
                        }

                        Console.WriteLine($"\nFordonet med registreringsnummer {sökRegNr} har tagits bort från Plats {hittatFordon}");
                        Console.WriteLine();
                        VisaParkering(parkeringen);
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("\nFordonet kunde inte hittas.");
                    }

                    break;
                }

            case 5:
                {
                    körMeny = false;
                    Console.WriteLine("\nAvslutar\n");
                    break;
                }

            default:
                {
                    Console.WriteLine("\nOgiltigt val\n");
                    break;
                }

        }

    }

    //Metod 1 - kollar om en specifik (inmatad) plats är ledig
    static bool ÄrLedig(string[] parkeringen, int plats)
    {
        return string.IsNullOrWhiteSpace(parkeringen[plats - 1]); //Platsnummer börjar på 1, men arrayens index börjar på 0
    }


    //Metod 2 - letar upp första lediga plats i parkeringen
    static int LedigPlats(string[] parkeringen)
    {
        for (int i = 0; i < parkeringen.Length; i++)
        {
            if (ÄrLedig(parkeringen, i + 1))
            {
                return i + 1;
            }
        }

        return -1;
    }

    //Metod 3 - söker efter registrerade fordon i arrayen via registreringsnummer
    static int SökFordon(string[] parkeringen, string regNr)
    {
        for (int i = 0; i < parkeringen.Length; i++)
        {
            if (!ÄrLedig(parkeringen, i + 1) && parkeringen[i].Contains(regNr)) //Hoppar över tomma platser först
            {
                return i + 1;
            }
        }

        return -1;
    }

    //Metod 4 - hittar en plats med en ensam MC om det finns
    static int MCPlats(string[] parkeringen)
    {
        for (int i = 0; i < parkeringen.Length; i++)
        {
            if (!ÄrLedig(parkeringen, i + 1) && parkeringen[i].StartsWith("MC#") && !parkeringen[i].Contains("|"))
            {
                return i + 1;
            }
        }

        return -1;
    }

    //Metod 5 - visar parkeringen med lediga och upptagna platser visuellt
    static void VisaParkering(string[] parkeringen)
    {
        int rader = 10;
        int kolumner = 10;

        for (int i = 0; i < rader; i++)
        {
            for (int j = 0; j < kolumner; j++)
            {
                int plats = i * kolumner + j + 1;

                if (ÄrLedig(parkeringen, plats))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(plats);
                }
                else if (parkeringen[plats - 1].StartsWith("MC#") && !parkeringen[plats - 1].Contains("|"))
                {
                    //Visar en "halv-ledig" MC-plats med en grön och en röd siffra
                    string deladPlats = plats.ToString("00");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(deladPlats.Substring(0, 1));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(deladPlats.Substring(1));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(plats);
                }
                Console.ResetColor();
                Console.Write("\t");
            }
            Console.WriteLine();
        }

    }
}