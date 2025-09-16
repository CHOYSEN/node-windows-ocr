import { join } from "node:path";
import { describe, expect, test } from "vitest";
import { recognizeBatchFromPath } from "../src";

describe("recognizeBatchFromPath", async () => {
  test("basic", async () => {
    const res = await recognizeBatchFromPath([join(__dirname, "./lorem.png")]);
    expect(res).toStrictEqual([
      {
        Language: "en-US",
        Result: {
          Lines: [
            {
              Text: "Lorem ipsum dolor sit amet,",
              Words: [
                {
                  BoundingRect: {
                    X: 49,
                    Y: 61,
                    Width: 235,
                    Height: 60,
                    Left: 49,
                    Top: 61,
                    Right: 284,
                    Bottom: 121,
                    IsEmpty: false,
                  },
                  Text: "Lorem",
                },
                {
                  BoundingRect: {
                    X: 328,
                    Y: 59,
                    Width: 226,
                    Height: 81,
                    Left: 328,
                    Top: 59,
                    Right: 554,
                    Bottom: 140,
                    IsEmpty: false,
                  },
                  Text: "ipsum",
                },
                {
                  BoundingRect: {
                    X: 595,
                    Y: 57,
                    Width: 200,
                    Height: 64,
                    Left: 595,
                    Top: 57,
                    Right: 795,
                    Bottom: 121,
                    IsEmpty: false,
                  },
                  Text: "dolor",
                },
                {
                  BoundingRect: {
                    X: 831,
                    Y: 59,
                    Width: 84,
                    Height: 62,
                    Left: 831,
                    Top: 59,
                    Right: 915,
                    Bottom: 121,
                    IsEmpty: false,
                  },
                  Text: "sit",
                },
                {
                  BoundingRect: {
                    X: 951,
                    Y: 66,
                    Width: 211,
                    Height: 64,
                    Left: 951,
                    Top: 66,
                    Right: 1162,
                    Bottom: 130,
                    IsEmpty: false,
                  },
                  Text: "amet,",
                },
              ],
            },
          ],
          Text: "Lorem ipsum dolor sit amet,",
          TextAngle: 0,
        },
      },
    ]);
  });
});
