using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace LyricsGame.Models
{
    public class InputProcessor
    {
        MusicDBContext db;
        public InputProcessor(MusicDBContext db)
        {
            this.db = db;
        }
        public void CutOff(LyricSegment segment, LyricSegment nextSegment)
        {
                segment.CutOffCount++;
                if (segment.CutOffCount % 5 == 0 && segment.CutOffCount < 20)
                {
                    segment.End += 0.5;
                    nextSegment.Start += 0.5;
                }
                else if(segment.CutOffCount >= 25)
                {
                    segment.End = nextSegment.End;

                    segment.LyricStats.Clear();
                    segment.LyricUsers.Clear();

                    segment.CutOffCount = 0;
                    segment.OnlyMusicCount = 0;

                    db.Lyrics.Remove(nextSegment);
                }
            db.SaveChanges();
        }

        public void NoLyrics(LyricSegment segment)
        {
            segment.OnlyMusicCount++;
            if (segment.OnlyMusicCount > 4)
                segment.Complete = true;

            db.SaveChanges();
        }

        public void Lyrics(LyricSegment segment, String input)
        {
            int userID = 0;

            //New entry in LyricUser table
            LyricsUser userEntry = new LyricsUser();
            userEntry.LyricSegmentID = segment.LyricSegmentID;
            userEntry.Lyrics = input;
            userEntry.UserID = userID;
            //userEntry.Time = Time;
            db.LyricUsers.Add(userEntry);

            //Strip out non alphanumeric characters including spaces
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            String inputMod = rgx.Replace(input, "");

           //Get all stat entries corresponding to segment
            IList<LyricsStats> lyricStatEntries = segment.LyricStats.ToList();

            bool newInput = true; //has the input been submitted before by another user
            int maxVotes = 0; //number of votes the top input has recieved
            int votes = 0; //number of total votes
            LyricsStats topStat = null; //top input

            //Go through all previously submitted inputs
            for (int i = 0; i < lyricStatEntries.Count(); i++)
            {
                String entryMod = rgx.Replace(lyricStatEntries[i].Lyrics, "");

                //If the stripped input mathes the stripped previous submission
                if(inputMod.Equals(entryMod))
                {
                    lyricStatEntries[i].Votes++; //increase votes for this submission
                    newInput = false; //the users submission was already in the table
                }

                //Is the analyzed input the top submission
                if (lyricStatEntries[i].Votes > maxVotes)
                {
                    maxVotes = lyricStatEntries[i].Votes;
                    topStat= lyricStatEntries[i];
                }
                
                votes += lyricStatEntries[i].Votes;
            }
            
            //If the user's submission does not already exist, create an entry for it
            if (newInput == true)
            {
                LyricsStats newEntry = new LyricsStats();
                newEntry.Lyrics = input;
                newEntry.LyricSegmentID = segment.LyricSegmentID;
                newEntry.Votes = 1;
                db.LyricStats.Add(newEntry);
            }
            //If the user's submission is already in a table check if the segment is complete
            else
            {
                double voteRatio = (double)(maxVotes/votes);
                if (voteRatio > 0.5 && maxVotes > 9)
                {
                    topStat.LyricSegment.Complete = true;

                    //If the segment is complete check if the song is complete
                    IList<LyricSegment> segments = db.Lyrics.Where(ls => ls.LyricSegmentID == topStat.LyricSegmentID && ls.Complete == false).ToList();
                    if (segments.Count() == 0)
                        topStat.LyricSegment.Music.Complete = true;
                       
                }
            }

            db.SaveChanges();

            return;
        }
    }
}