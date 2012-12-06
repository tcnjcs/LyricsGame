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
        public bool CutOff(LyricSegment segment, LyricSegment nextSegment)
        {
            bool match = false;
            if (segment.CutOffCount != 0)
                match = true;

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

            return match;
        }

        public bool NoLyrics(LyricSegment segment)
        {
            bool match = false;
            if (segment.OnlyMusicCount != 0)
                match = true;
            segment.OnlyMusicCount++;
            if (segment.OnlyMusicCount > 4)
            {
                segment.Complete = true;
                songCompletion(segment.Music);
            }

            db.SaveChanges();

            return match;
        }

        public bool Lyrics(LyricSegment segment, String input, double startTime, UserProfile activeUser)
        {
            int userID = 0;
            TimeSpan now = DateTime.UtcNow - new DateTime(1970, 1, 1);
            bool match = false;
           
            
            //New entry in LyricUser table
            LyricsUser userEntry = new LyricsUser();
            userEntry.LyricSegmentID = segment.LyricSegmentID;
            userEntry.Lyrics = input;
            userEntry.UserID = userID;
            userEntry.Time = (int)(now.TotalSeconds-startTime);
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
            LyricsStats userSub = null;
            //Go through all previously submitted inputs
            for (int i = 0; i < lyricStatEntries.Count(); i++)
            {
                String entryMod = rgx.Replace(lyricStatEntries[i].Lyrics, "");

                //If the stripped input mathes the stripped previous submission
                if(inputMod.Equals(entryMod))
                {
                    lyricStatEntries[i].Votes++; //increase votes for this submission
                    userSub = lyricStatEntries[i];
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
                newEntry.Available = false;
                newEntry.Votes = 1;
                userSub = newEntry;
                db.LyricStats.Add(newEntry);


            }
            //If the user's submission is already in a table check if the segment is complete
            else
            {
                double voteRatio = (double)(maxVotes)/votes;
                if (voteRatio > 0.5 && maxVotes > 9)
                {
                    topStat.LyricSegment.Complete = true;

                    //If the segment is complete check if the song is complete
                    songCompletion(topStat.LyricSegment.Music);
                }

                //Check if user input is in top 5 submissions
                List<LyricsStats> topResults = (from p in db.LyricStats where p.LyricSegmentID == segment.LyricSegmentID orderby p.Votes descending select p).Take(5).ToList();
                for (int i = 0; i < topResults.Count(); i++)
                {
                    if (topResults.Count() != 0)
                    {
                        String entryMod = rgx.Replace(lyricStatEntries[i].Lyrics, "");
                        if (entryMod.Equals(inputMod))
                            match = true;
                    }
                }
                
            }

            List<UserSegmentVotes> otherVote = db.UserSegmentVotes.Where(us => us.LyricSegmentID == segment.LyricSegmentID).ToList();
            if (otherVote.Count() == 0)
            {
                UserSegmentVotes newEntryUser = new UserSegmentVotes();
                newEntryUser.LyricSegmentID = segment.LyricSegmentID;
                newEntryUser.LyricsStatsID = userSub.LyricsStatsID;
                newEntryUser.UserID = activeUser.UserId;
                db.UserSegmentVotes.Add(newEntryUser);
            }
            else
            {
                otherVote.First().LyricsStatsID = userSub.LyricsStatsID;
            }

            //Mark all lyricstats meeting threshold as available to be voted on
            for (int i = 0; i < lyricStatEntries.Count(); i++)
            {
                double statRatio = (double)(lyricStatEntries[i].Votes) / votes;
                if (lyricStatEntries[i].Votes > 9 &&  statRatio > 0.5)
                    lyricStatEntries[i].Available = true;
                else
                    lyricStatEntries[i].Available = false;

            }

            db.SaveChanges();

            return match;
        }

        private void songCompletion(Music song)
        {
            IList<LyricSegment> segments = db.Lyrics.Where(ls => ls.MusicID  == song.MusicID && ls.Complete == false).ToList();
            if (segments.Count() == 0)
                song.Complete = true;
        }
    }
}